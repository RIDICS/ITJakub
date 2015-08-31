using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class AdminHostViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IErrorService m_errorService;
        private string m_groupName;
        private AdminBaseViewModel m_adminViewModel;
        private bool m_isCommandBarOpen;
        private bool m_isChatSupported;
        private bool m_isChatDisplayed;
        private SupportAppBaseViewModel m_chatApplicationViewModel;

        public AdminHostViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;
            InitCommands();
            LoadData();
        }
        
        private void InitCommands()
        {
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void GoBack()
        {
            // TODO unregister all polling requests
            m_navigationService.GoBack();
        }

        private void LoadData()
        {
            m_dataService.GetCurrentGroupId((groupId, type) =>
            {
                LoadGroupDetails(groupId);
            });
        }

        private void LoadGroupDetails(long groupId)
        {
            m_dataService.GetGroupDetails(groupId, (groupInfo, exception) =>
            {
                if (exception != null)
                {
                    m_errorService.ShowConnectionError();
                    return;
                }

                GroupName = groupInfo.GroupName;
            });
        }

        public RelayCommand GoBackCommand { get; private set; }

        public string GroupName
        {
            get { return m_groupName; }
            set
            {
                m_groupName = value;
                RaisePropertyChanged();
            }
        }

        public AdminBaseViewModel AdminViewModel
        {
            get { return m_adminViewModel; }
            set
            {
                m_adminViewModel = value;
                RaisePropertyChanged();
            }
        }

        public SupportAppBaseViewModel ChatApplicationViewModel
        {
            get { return m_chatApplicationViewModel; }
            set
            {
                m_chatApplicationViewModel = value;
                RaisePropertyChanged();
            }
        }

        public bool IsCommandBarOpen
        {
            get { return m_isCommandBarOpen; }
            set
            {
                m_isCommandBarOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool IsChatSupported
        {
            get { return m_isChatSupported; }
            set
            {
                m_isChatSupported = value;
                RaisePropertyChanged();
            }
        }

        public bool IsChatDisplayed
        {
            get { return m_isChatDisplayed; }
            set
            {
                m_isChatDisplayed = value;
                RaisePropertyChanged();
            }
        }
    }
}
