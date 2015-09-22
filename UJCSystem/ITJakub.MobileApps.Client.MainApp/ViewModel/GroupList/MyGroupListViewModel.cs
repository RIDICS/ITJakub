using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
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
        

        public MyGroupListViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;

            InitCommands();
            InitViewModels();
            LoadData();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(m_navigationService.GoBack);
            RefreshListCommand = new RelayCommand(LoadData);
        }

        private void InitViewModels()
        {
            ConnectToGroupViewModel = new ConnectToGroupViewModel(m_dataService, m_errorService, LoadData);
        }

        private void LoadData()
        {
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
            });
        }

        public RelayCommand GoBackCommand { get; private set; }
        
        public RelayCommand RefreshListCommand { get; private set; }
        
        public ConnectToGroupViewModel ConnectToGroupViewModel { get; set; }


        protected override void OpenGroup(GroupInfoViewModel group)
        {
            if (group == null)
                return;

            //var ownCurrentGroup = m_ownedGroupIds.Contains(group.GroupId);
            //m_dataService.SetCurrentGroup(group.GroupId, ownCurrentGroup ? GroupType.Owner : GroupType.Member);
            m_dataService.SetCurrentGroup(group.GroupId, GroupType.Member); // TODO determine type

            m_navigationService.Navigate<ApplicationHostView>();
        }

        public override bool IsTeacherView { get { return false; } }
    }
}