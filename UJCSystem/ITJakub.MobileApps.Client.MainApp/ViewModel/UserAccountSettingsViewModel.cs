using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Login.UserMenu;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class UserAccountSettingsViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IErrorService m_errorService;
        private bool m_isSettingsFlyoutOpen;
        private LoggedUserViewModel m_userInfoViewModel;
        private bool m_isUserLoggedIn;
        private bool m_isSwitchToTeacherError;
        private bool m_switchingInProgresss;

        public UserAccountSettingsViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;

            IsSettingsFlyoutOpen = true;
            LogOutCommand = new RelayCommand(LogOut);
            SwitchToTeacherCommand = new RelayCommand(SwitchToTeacher);
            m_dataService.GetLoggedUserInfo(true, user => UserInfoViewModel = user);
        }
        
        public RelayCommand LogOutCommand { get; private set; }

        public RelayCommand SwitchToTeacherCommand { get; set; }

        public LoggedUserViewModel UserInfoViewModel
        {
            get { return m_userInfoViewModel; }
            set
            {
                m_userInfoViewModel = value;
                RaisePropertyChanged();
                IsUserLoggedIn = value != null;
                RaisePropertyChanged(() => IsSwitchToTeacherVisible);
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

        public bool IsSwitchToTeacherVisible
        {
            get { return m_isUserLoggedIn && m_userInfoViewModel.UserRole != UserRoleContract.Teacher; }
        }

        public string InstitutionCode { get; set; }

        public bool IsSwitchToTeacherError
        {
            get { return m_isSwitchToTeacherError; }
            set
            {
                m_isSwitchToTeacherError = value;
                RaisePropertyChanged();
            }
        }

        public bool SwitchingInProgresss
        {
            get { return m_switchingInProgresss; }
            set
            {
                m_switchingInProgresss = value;
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

        private void SwitchToTeacher()
        {
            if (string.IsNullOrWhiteSpace(InstitutionCode))
            {
                IsSwitchToTeacherError = true;
                return;
            }

            IsSwitchToTeacherError = false;
            SwitchingInProgresss = true;
            m_dataService.PromoteUserToTeacherRole(m_userInfoViewModel.UserId, InstitutionCode,
                (success, exception) =>
                {
                    SwitchingInProgresss = false;
                    if (exception != null)
                    {
                        //TODO handle connection error
                        IsSwitchToTeacherError = true;
                        return;
                    }

                    IsSwitchToTeacherError = !success;
                    if (success)
                    {
                        LogOut();
                        m_errorService.ShowError(
                            "Přepnutí na učitele proběhlo v pořádku. Prosím přihlašte se znovu.",
                            "Přepnutí na učitele");
                    }
                });
        }
    }
}
