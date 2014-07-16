using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.MainApp.Enum;

namespace ITJakub.MobileApps.MainApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private readonly RelayCommand m_facebookLoginCommand;
        private readonly RelayCommand m_googleLoginCommand;
        private readonly RelayCommand m_liveIdCommand;
        private IDataService m_dataService;

        /// <summary>
        /// Initializes a new instance of the LoginViewModel class.
        /// </summary>
        public LoginViewModel(IDataService dataService)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            m_dataService = dataService;
            m_liveIdCommand = new RelayCommand(LiveIdLogin);
            m_facebookLoginCommand = new RelayCommand(FacebookLogin);
            m_googleLoginCommand = new RelayCommand(GoogleLogin);
        }

        public RelayCommand FacebookLoginCommand
        {
            get { return m_facebookLoginCommand; }
        }

        public string Message { get; set; }

        public RelayCommand GoogleLoginCommand
        {
            get { return m_googleLoginCommand; }
        }

        public RelayCommand LiveIdLoginCommand
        {
            get { return m_liveIdCommand; }
        }

        private void FacebookLogin()
        {
            m_dataService.LoginAsync(LoginProvider.Facebook, (info, exception) =>
            {
                Message = String.Format("{0} {1} {2} {3} {4}", info.Success, info.Email, info.FirstName, info.LastName, info.AccessToken);
                RaisePropertyChanged(() => Message);
            });
        }

        private void GoogleLogin()
        {
            m_dataService.LoginAsync(LoginProvider.Google, (info, exception) =>
            {
                Message = String.Format("{0} {1} {2} {3} {4}", info.Success, info.Email, info.FirstName, info.LastName, info.AccessToken);
                RaisePropertyChanged(() => Message);
            });
        }

        private void LiveIdLogin()
        {
            m_dataService.LoginAsync(LoginProvider.LiveId, (info, exception) =>
            {
                Message = String.Format("{0} {1} {2} {3} {4}", info.Success, info.Email, info.FirstName, info.LastName, info.AccessToken);
                RaisePropertyChanged(() => Message);
            });
        }
    }
}