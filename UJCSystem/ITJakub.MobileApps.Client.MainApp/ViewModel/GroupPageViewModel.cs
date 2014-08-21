using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GroupPageViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private string m_groupName;
        private string m_groupCode;
        private ObservableCollection<GroupMemberViewModel> m_memberList;
        private DateTime m_createTime;
        private AppInfoViewModel m_selectedApplicationInfo;
        private GroupInfoViewModel m_groupInfo;

        /// <summary>
        /// Initializes a new instance of the GroupPageViewModel class.
        /// </summary>
        public GroupPageViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            Messenger.Default.Register<OpenGroupMessage>(this, message =>
            {
                LoadData(message.Group);
                Messenger.Default.Unregister<OpenGroupMessage>(this);
            });

            InitCommands();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(() => m_navigationService.GoBack());
            SelectAppCommand = new RelayCommand(SelectApplication);
            SelectTaskCommand = new RelayCommand(SelectTask);
        }

        private void LoadData(GroupInfoViewModel group)
        {
            GroupInfo = group;
            //GroupName = group.GroupName;
            GroupCode = group.GroupCode;
            CreateTime = group.CreateTime;
            MemberList = group.Members;
        }

        public GroupInfoViewModel GroupInfo
        {
            get { return m_groupInfo; }
            set
            {
                m_groupInfo = value;
                RaisePropertyChanged();
            }
        }

        public string GroupName
        {
            get { return m_groupName; }
            set
            {
                m_groupName = value;
                RaisePropertyChanged();
            }
        }

        public string GroupCode
        {
            get { return m_groupCode; }
            set { m_groupCode = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<GroupMemberViewModel> MemberList
        {
            get { return m_memberList; }
            set { m_memberList = value; RaisePropertyChanged(); }
        }

        public DateTime CreateTime
        {
            get { return m_createTime; }
            set
            {
                m_createTime = value;
                RaisePropertyChanged();
            }
        }

        public AppInfoViewModel SelectedApplicationInfo
        {
            get { return m_selectedApplicationInfo; }
            set
            {
                m_selectedApplicationInfo = value;
                RaisePropertyChanged();
            }
        }

        public string SearchText { get; set; }

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand SelectAppCommand { get; private set; }

        public RelayCommand SelectTaskCommand { get; private set; }

        public RelayCommand SearchCommand { get; set; }
        

        private void SelectApplication()
        {
            m_navigationService.Navigate(typeof(ApplicationSelectionView));
            Messenger.Default.Register<ApplicationSelectedMessage>(this, message =>
            {
                SelectedApplicationInfo = message.AppInfo;
                Messenger.Default.Unregister<ApplicationSelectedMessage>(this);
            });
        }

        private void SelectTask()
        {
            throw new NotImplementedException();
        }
    }
}