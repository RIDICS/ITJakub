using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Communication;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.View.Login;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.Login
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly IErrorService m_errorService;
        private bool m_loggingIn;

        public LoginViewModel(IDataService dataService, INavigationService navigationService, IErrorService errorService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            m_errorService = errorService;
            LoggingIn = false;
            LoadInitData();

            GoBackCommand = new RelayCommand(m_navigationService.GoBack);
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(ItemClick);
            RegistrationCommand = new RelayCommand(() => m_navigationService.Navigate<RegistrationView>());
        }


        public ObservableCollection<LoginProviderViewModel> LoginProviders { get; set; }

        public RelayCommand<ItemClickEventArgs> ItemClickCommand { get; private set; }

        public RelayCommand RegistrationCommand { get; private set; }
        
        public RelayCommand GoBackCommand { get; private set; }

        public bool LoggingIn
        {
            get { return m_loggingIn; }
            set
            {
                m_loggingIn = value;
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

        private void Login(AuthProvidersContract loginProviderType)
        {
            LoggingIn = true;
            m_dataService.Login(loginProviderType, (loginResult, exception) =>
            {
                LoggingIn = false;
                if (exception != null)
                {
                    if (exception is UserNotRegisteredException)
                        new MessageDialog("Pro přihlášení do aplikace je nutné se nejdříve registrovat.", "Uživatel není registrován").ShowAsync();
                    else
                        m_errorService.ShowConnectionError();

                    return;
                }

                if (loginResult)
                    m_navigationService.Navigate<GroupListView>();
            });
        }
    }
}