using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class MyGroupListViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IErrorService m_errorService;
        private ObservableCollection<GroupInfoViewModel> m_groupList;
        private bool m_isGroupListEmpty;
        private bool m_loading;

        public MyGroupListViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;

            ConnectToGroupViewModel = new ConnectToGroupViewModel(dataService, errorService);

            InitCommands();
            LoadData();
        }

        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(m_navigationService.GoBack);
            GroupClickCommand = new RelayCommand<ItemClickEventArgs>(OpenGroup);
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

                GroupList = new ObservableCollection<GroupInfoViewModel>(groupList);
            });
        }

        public RelayCommand GoBackCommand { get; private set; }

        public RelayCommand<ItemClickEventArgs> GroupClickCommand { get; set; }

        public ConnectToGroupViewModel ConnectToGroupViewModel { get; set; }

        public ObservableCollection<GroupInfoViewModel> GroupList
        {
            get { return m_groupList; }
            set
            {
                m_groupList = value;
                RaisePropertyChanged();
                IsGroupListEmpty = m_groupList.Count == 0;
            }
        }

        public bool IsGroupListEmpty
        {
            get { return m_isGroupListEmpty; }
            set
            {
                m_isGroupListEmpty = value;
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

        
        private void OpenGroup(ItemClickEventArgs args)
        {
            var group = args.ClickedItem as GroupInfoViewModel;
            if (group == null)
                return;
            
            //var ownCurrentGroup = m_ownedGroupIds.Contains(group.GroupId);
            //m_dataService.SetCurrentGroup(group.GroupId, ownCurrentGroup ? GroupType.Owner : GroupType.Member);
            m_dataService.SetCurrentGroup(group.GroupId, GroupType.Member); // TODO determine type

            m_navigationService.Navigate<ApplicationHostView>();
        }

    }
}