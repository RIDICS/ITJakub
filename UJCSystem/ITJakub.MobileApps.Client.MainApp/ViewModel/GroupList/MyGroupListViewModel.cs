using System;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.Service.Polling;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class MyGroupListViewModel : GroupListViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IErrorService m_errorService;
        private readonly IMainPollingService m_pollingService;


        public MyGroupListViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService, IMainPollingService pollingService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;
            m_pollingService = pollingService;

            InitCommands();
            InitViewModels();
            LoadData();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(() =>
            {
                m_pollingService.UnregisterAll();
                m_navigationService.GoBack();
            });
            RefreshListCommand = new RelayCommand(LoadData);
        }

        private void InitViewModels()
        {
            ConnectToGroupViewModel = new ConnectToGroupViewModel(m_dataService, m_errorService, LoadData);
        }

        private void LoadData()
        {
            m_pollingService.Unregister(UpdatePollingInterval, GroupUpdate);

            Loading = true;
            m_dataService.GetGroupsForCurrentUser((groupList, exception) =>
            {
                Loading = false;
                if (exception != null)
                {
                    m_errorService.ShowConnectionWarning();
                    return;
                }

                m_completeGroupList = groupList;
                DisplayGroupList();

                m_pollingService.RegisterForGroupsUpdate(UpdatePollingInterval, m_completeGroupList, GroupUpdate);
            });
        }

        public RelayCommand GoBackCommand { get; private set; }
        
        public RelayCommand RefreshListCommand { get; private set; }
        
        public ConnectToGroupViewModel ConnectToGroupViewModel { get; set; }


        protected override void OpenGroup(GroupInfoViewModel group)
        {
            if (group == null)
                return;

            m_dataService.GetLoggedUserInfo(false, user =>
            {
                var isUserGroupOwner = group.AuthorId == user.UserId;
                var groupType = isUserGroupOwner ? GroupType.Owner : GroupType.Member;

                m_pollingService.UnregisterAll();
                m_dataService.SetCurrentGroup(group.GroupId, groupType);
                m_navigationService.Navigate<ApplicationHostView>();
            });
        }

        public override bool IsTeacherView { get { return false; } }

        private void GroupUpdate(Exception exception)
        {
            if (exception != null)
                m_errorService.ShowConnectionWarning();
            else
                m_errorService.HideWarning();
        }
    }
}