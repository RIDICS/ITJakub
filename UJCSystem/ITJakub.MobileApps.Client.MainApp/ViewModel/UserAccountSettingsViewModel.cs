using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Login.UserMenu;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class UserAccountSettingsViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private bool m_isSettingsFlyoutOpen;
        private LoggedUserViewModel m_userInfoViewModel;
        private bool m_isUserLoggedIn;

        public UserAccountSettingsViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;

            IsSettingsFlyoutOpen = true;
            LogOutCommand = new RelayCommand(LogOut);
            m_dataService.GetLoggedUserInfo(true, user => UserInfoViewModel = user);
        }

        public RelayCommand LogOutCommand { get; private set; }

        public LoggedUserViewModel UserInfoViewModel
        {
            get { return m_userInfoViewModel; }
            set
            {
                m_userInfoViewModel = value;
                RaisePropertyChanged();
                IsUserLoggedIn = value != null;
            }
        }
        
        public bool IsSettingsFlyoutOpen
        {
            get { return m_isSettingsFlyoutOpen; }
            set
            {
                m_isSettingsFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool IsUserLoggedIn
        {
            get { return m_isUserLoggedIn; }
            set
            {
                m_isUserLoggedIn = value;
                RaisePropertyChanged();
            }
        }

        private void LogOut()
        {
            MessengerInstance.Send(new LogOutMessage());
            m_dataService.LogOut();
            m_navigationService.GoHome();
            IsSettingsFlyoutOpen = false;
        }
    }
}
