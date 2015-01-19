using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Login.UserMenu;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class GroupListViewModel : ViewModelBase
    {
        private const PollingInterval UpdatePollingInterval = PollingInterval.Medium;

        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IMainPollingService m_pollingService;
        
        private UserRoleContract m_userRole;
        private bool m_commandBarOpen;
        private ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>> m_groupList;
        private bool m_noGroupExist;
        private GroupInfoViewModel m_selectedGroup;
        private bool m_loading;
        private ObservableCollection<GroupInfoViewModel> m_groups;
        private bool m_isDeleteFlyoutOpen;

        /// <summary>
        ///     Initializes a new instance of the GroupListViewModel class.
        /// </summary>
        public GroupListViewModel(IDataService dataService, INavigationService navigationService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_pollingService = pollingService;
            GroupList = new ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>>();
            NoGroupExist = false;
            m_userRole = UserRoleContract.Student;
            
            Messenger.Default.Register<LogOutMessage>(this, message =>
            {
                m_pollingService.UnregisterAll();
                Messenger.Default.Unregister(this);
            });

            InitViewModels();
            InitCommands();
            LoadData();
        }

        public ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>> GroupList
        {
            get { return m_groupList; }
            set
            {
                m_groupList = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<ItemClickEventArgs> GroupClickCommand { get; private set; }

        public RelayCommand RefreshListCommand { get; private set; }

        public RelayCommand ConnectCommand { get; private set; }

        public RelayCommand OpenTaskEditorCommand { get; private set; }
        
        public bool CommandBarOpen
        {
            get { return m_commandBarOpen; }
            set
            {
                m_commandBarOpen = value;
                RaisePropertyChanged();
            }
        }

        public GroupInfoViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set
            {
                m_selectedGroup = value;
                CommandBarOpen = value != null;
                DeleteGroupViewModel.SelectedGroup = value;

                RaisePropertyChanged();
                RaisePropertyChanged(() => ConnectButtonVisibility);
                RaisePropertyChanged(() => TeachersSecondaryButtonVisibility);
            }
        }

        public Visibility ConnectButtonVisibility
        {
            get { return SelectedGroup != null ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility TeachersSecondaryButtonVisibility
        {
            get
            {
                bool isVisible = m_userRole == UserRoleContract.Teacher && SelectedGroup != null;
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility TeachersButtonVisibility
        {
            get { return m_userRole == UserRoleContract.Teacher ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool NoGroupExist
        {
            get { return m_noGroupExist; }
            set
            {
                m_noGroupExist = value;
                RaisePropertyChanged();
            }
        }

        public bool Loading
        {
            get { return m_loading; }
            set
            {
                m_loading = value;
                RaisePropertyChanged();
            }
        }

        public bool IsDeleteFlyoutOpen
        {
            get { return m_isDeleteFlyoutOpen; }
            set
            {
                m_isDeleteFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public ConnectToGroupViewModel ConnectToGroupViewModel { get; set; }

        public CreateGroupViewModel CreateNewGroupViewModel { get; set; }

        public DeleteGroupViewModel DeleteGroupViewModel { get; set; }

        public RelayCommand GoBackCommand { get; private set; }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(m_navigationService.GoBack);
            GroupClickCommand = new RelayCommand<ItemClickEventArgs>(GroupClick);
            ConnectCommand = new RelayCommand(() => OpenGroup(SelectedGroup));
            RefreshListCommand = new RelayCommand(LoadData);
            OpenTaskEditorCommand = new RelayCommand(() => m_navigationService.Navigate(typeof(OwnedTaskListView)));
        }
        
        private void InitViewModels()
        {
            ConnectToGroupViewModel = new ConnectToGroupViewModel(m_dataService, LoadData);
            CreateNewGroupViewModel = new CreateGroupViewModel(m_dataService, LoadData);
            DeleteGroupViewModel = new DeleteGroupViewModel(m_dataService, LoadData);
        }

        private void LoadData()
        {
            m_pollingService.Unregister(UpdatePollingInterval, GroupUpdate);
            Loading = true;

            m_dataService.GetGroupList((groupList, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;

                m_groups = groupList;
                var groupedList = groupList.GroupBy(group => group.GroupType).OrderBy(group => group.Key);
                GroupList = new ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>>(groupedList);
                NoGroupExist = groupList.Count == 0;

                m_pollingService.RegisterForGroupsUpdate(UpdatePollingInterval, m_groups, GroupUpdate);
            });

            m_dataService.GetLoggedUserInfo((info, exception) =>
            {
                if (exception != null)
                    return;
                m_userRole = info.UserRole;
                m_userRole = UserRoleContract.Teacher;  //TODO for debug
                RaisePropertyChanged(() => TeachersButtonVisibility);
                RaisePropertyChanged(() => TeachersSecondaryButtonVisibility);
            });
        }

        private void GroupUpdate(Exception exception)
        {
            if (exception != null)
                return;
        }

        private void GroupClick(ItemClickEventArgs args)
        {
            var group = args.ClickedItem as GroupInfoViewModel;
            OpenGroup(group);
        }

        private void OpenGroup(GroupInfoViewModel group)
        {
            if (group != null)
            {
                m_dataService.SetCurrentGroup(group.GroupId);
                m_pollingService.Unregister(UpdatePollingInterval, GroupUpdate);

                var viewType = group.GroupType == GroupType.Member
                    ? typeof (ApplicationHostView)
                    : typeof (GroupPageView);
                m_navigationService.Navigate(viewType);
                Messenger.Default.Unregister(this);
            }
        }
    }
}