using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Core.Error;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.View.Login;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private bool m_loggingIn;
        private Visibility m_loginDialogVisibility;

        public LoginViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            LoggingIn = false;
            LoadInitData();
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(ItemClick);
            RegistrationCommand = new RelayCommand(() => m_navigationService.Navigate(typeof (RegistrationView)));
        }


        public ObservableCollection<LoginProviderViewModel> LoginProviders { get; set; }

        public RelayCommand<ItemClickEventArgs> ItemClickCommand { get; private set; }

        public RelayCommand RegistrationCommand { get; private set; }

        public Visibility LoginDialogVisibility
        {
            get { return m_loginDialogVisibility; }
            set
            {
                m_loginDialogVisibility = value;
                RaisePropertyChanged();
            }
        }

        public bool LoggingIn
        {
            get { return m_loggingIn; }
            set
            {
                m_loggingIn = value;
                LoginDialogVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged();
            }
        }

        private void LoadInitData()
        {
            LoginProviders = new ObservableCollection<LoginProviderViewModel>();
            m_dataService.GetLoginProviders((loginproviders, exception) =>
            {
                if (exception != null)
                    return;

                foreach (LoginProviderViewModel loginProvider in loginproviders)
                {
                    LoginProviders.Add(loginProvider);
                }
            });
        }

        private void ItemClick(ItemClickEventArgs args)
        {
            var item = args.ClickedItem as LoginProviderViewModel;
            if (item == null)
                return;

            Login(item.LoginProviderType);
        }

        private void Login(LoginProviderType loginProviderType)
        {
            LoggingIn = true;
            m_dataService.Login(loginProviderType, (info, exception) =>
            {
                LoggingIn = false;
                if (exception != null)
                {
                    if (exception is UserNotRegisteredException)
                        new MessageDialog("Pro pøihlášení do aplikace je nutné se nejdøíve registrovat.", "Uživatel není registrován").ShowAsync();
                    return;
                }
                if (info != null)
                {
                    if (info.Success)
                        m_navigationService.Navigate(typeof (GroupListView));
                }
            });
        }
    }
}