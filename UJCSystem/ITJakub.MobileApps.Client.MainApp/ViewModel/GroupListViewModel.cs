using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    /// <summary>
    ///     This class contains properties that a View can data bind to.
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class GroupListViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private UserRole m_userRole;
        private readonly INavigationService m_navigationService;
        private bool m_commandBarOpen;
        private RelayCommand m_connectCommand;
        private RelayCommand m_connectToGroupCommand;
        private string m_connectToGroupCode;
        private RelayCommand m_createNewGroupCommand;
        private RelayCommand m_deleteGroupCommand;
        private string m_deleteMessage;
        private RelayCommand<ItemClickEventArgs> m_groupClickCommand;
        private ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>> m_groupList;
        private string m_newGroupName;
        private bool m_noGroupExist;
        private RelayCommand m_refreshListCommand;
        private GroupInfoViewModel m_selectedGroup;
        private bool m_loading;

        /// <summary>
        ///     Initializes a new instance of the GroupListViewModel class.
        /// </summary>
        public GroupListViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            GroupList = new ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>>();
            NoGroupExist = false;
            m_userRole = UserRole.Student;

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

        public RelayCommand<ItemClickEventArgs> GroupClickCommand
        {
            get { return m_groupClickCommand; }
        }

        public RelayCommand CreateNewGroupCommand
        {
            get { return m_createNewGroupCommand; }
        }

        public RelayCommand DeleteGroupCommand
        {
            get { return m_deleteGroupCommand; }
        }

        public RelayCommand RefreshListCommand
        {
            get { return m_refreshListCommand; }
        }

        public string ConnectToGroupCode
        {
            get { return m_connectToGroupCode; }
            set
            {
                m_connectToGroupCode = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ConnectToGroupCommand
        {
            get { return m_connectToGroupCommand; }
        }

        public string NewGroupName
        {
            get { return m_newGroupName; }
            set
            {
                m_newGroupName = value;
                RaisePropertyChanged();
            }
        }

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
                RaisePropertyChanged();
                RaisePropertyChanged(() => ConnectButtonVisibility);
                RaisePropertyChanged(() => TeachersSecondaryButtonVisibility);
                if (value != null)
                    DeleteMessage = string.Format("Chystáte se odstranit skupinu \"{0}\"", value.GroupName);
            }
        }

        public RelayCommand ConnectCommand
        {
            get { return m_connectCommand; }
        }

        public Visibility ConnectButtonVisibility
        {
            get { return SelectedGroup != null ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility TeachersSecondaryButtonVisibility
        {
            get
            {
                bool isVisible = m_userRole == UserRole.Teacher && SelectedGroup != null;
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility TeachersButtonVisibility
        {
            get { return m_userRole == UserRole.Teacher ? Visibility.Visible : Visibility.Collapsed; }
        }

        public string DeleteMessage
        {
            get { return m_deleteMessage; }
            set
            {
                m_deleteMessage = value;
                RaisePropertyChanged();
            }
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

        private void InitCommands()
        {
            m_groupClickCommand = new RelayCommand<ItemClickEventArgs>(GroupClick);
            m_connectCommand = new RelayCommand(() => OpenGroup(SelectedGroup));
            m_createNewGroupCommand = new RelayCommand(CreateNewGroup);
            m_refreshListCommand = new RelayCommand(LoadData);
            m_connectToGroupCommand = new RelayCommand(ConnectToGroup);
        }

        private void ConnectToGroup()
        {
            if (ConnectToGroupCode == string.Empty)
                return;

            m_dataService.ConnectToGroup(ConnectToGroupCode, exception =>
            {
                if (exception != null)
                    return;
                new MessageDialog("Připojeno").ShowAsync();
                LoadData();
            });
        }

        private void CreateNewGroup()
        {
            if (NewGroupName == string.Empty)
                return;

            m_dataService.CreateNewGroup(NewGroupName, (result, exception) =>
            {
                if (exception != null)
                    return;
                new MessageDialog(result.EnterCode, "Nová skupina vytvořena").ShowAsync();
                NewGroupName = string.Empty;
                LoadData();
            });
        }

        private void LoadData()
        {
            Loading = true;
            m_dataService.GetGroupList((groupList, exception) =>
            {
                Loading = false;
                if (exception != null)
                    return;
                var groupedList = groupList.GroupBy(group => group.GroupType).OrderBy(group => group.Key);
                GroupList = new ObservableCollection<IGrouping<GroupType, GroupInfoViewModel>>(groupedList);
                NoGroupExist = groupList.Count == 0;
                
                foreach (var group in groupList)
                {
                    LoadGroupMembers(group);
                }
            });
            m_dataService.GetLoggedUserInfo((info, exception) =>
            {
                if (exception != null)
                    return;
                m_userRole = info.UserRole;
                m_userRole = UserRole.Teacher;  //TODO for debug
                RaisePropertyChanged(() => TeachersButtonVisibility);
                RaisePropertyChanged(() => TeachersSecondaryButtonVisibility);
            });
        }

        private void LoadGroupMembers(GroupInfoViewModel group)
        {
            m_dataService.GetGroupMembers(group.GroupId, (members, exception) =>
            {
                if (exception != null)
                    return;

                group.Members = members;
            });
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
                var viewType = group.GroupType == GroupType.Member
                    ? typeof (ApplicationHostView)
                    : typeof (GroupPageView);
                m_navigationService.Navigate(viewType);
                Messenger.Default.Send(new OpenGroupMessage {Group = group});
            }
        }
    }
}