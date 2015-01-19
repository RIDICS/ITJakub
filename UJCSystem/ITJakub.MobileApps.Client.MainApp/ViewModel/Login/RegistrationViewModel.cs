using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Communication;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.Login
{
    public class RegistrationViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private Visibility m_loginDialogVisibility;
        private bool m_registrationInProgress;

        public RegistrationViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            RegistrationInProgress = false;

            GoBackCommand = new RelayCommand(m_navigationService.GoBack);
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(ItemClick);
            LoadInitData();
        }

        private void LoadInitData()
        {
            LoginProviders = new ObservableCollection<LoginProviderViewModel>();
            m_dataService.GetLoginProviders((list, exception) =>
            {
                if (exception != null)
                    return;

                foreach (var loginProvider in list)
                {
                    LoginProviders.Add(loginProvider);
                }
            });
        }

        public ObservableCollection<LoginProviderViewModel> LoginProviders { get; set; }

        public RelayCommand<ItemClickEventArgs> ItemClickCommand { get; private set; }

        public RelayCommand GoBackCommand { get; private set; }

        public Visibility LoginDialogVisibility
        {
            get { return m_loginDialogVisibility; }
            set
            {
                m_loginDialogVisibility = value;
                RaisePropertyChanged();
            }
        }

        public bool RegistrationInProgress
        {
            get { return m_registrationInProgress; }
            set
            {
                m_registrationInProgress = value;
                LoginDialogVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged();
            }
        }

        private void ItemClick(ItemClickEventArgs args)
        {
            var item = args.ClickedItem as LoginProviderViewModel;
            if (item == null)
                return;

            Register(item.LoginProviderType);
        }

        private void Register(AuthProvidersContract loginProviderType)
        {
            RegistrationInProgress = true;
            m_dataService.CreateUser(loginProviderType, (createUserResult, exception) =>
            {
                RegistrationInProgress = false;
                if (exception != null)
                {
                    if (exception is UserAlreadyRegisteredException)
                        new MessageDialog("Tento uživatelský účet už byl v minulosti v systému zaregistrován. Pro pokračování využijte předchozí obrazovku \"Přihlášení\".", "Uživatel je registrovaný").ShowAsync();

                    return;
                }

                if (createUserResult)
                    m_navigationService.Navigate(typeof(GroupListView));
            });
        }
    }
}