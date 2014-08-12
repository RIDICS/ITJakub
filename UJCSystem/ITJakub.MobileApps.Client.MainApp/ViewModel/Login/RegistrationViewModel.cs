using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.Login
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RegistrationViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private RelayCommand<ItemClickEventArgs> m_itemClickCommand;
        private Visibility m_loginDialogVisibility;
        private bool m_registrationInProgress;

        /// <summary>
        /// Initializes a new instance of the RegistrationViewModel class.
        /// </summary>
        public RegistrationViewModel(IDataService dataService, INavigationService navigationService)
        {
            m_dataService = dataService;
            m_navigationService = navigationService;
            RegistrationInProgress = false;
            m_itemClickCommand = new RelayCommand<ItemClickEventArgs>(ItemClick);
            LoadInitData();
        }

        private void LoadInitData()
        {
            m_dataService.GetLoginProviders((list, exception) =>
            {
                if (exception != null)
                    return;

                LoginProviders = new ObservableCollection<LoginProviderViewModel>(list);
            });
        }

        public RelayCommand<ItemClickEventArgs> ItemClickCommand
        {
            get { return m_itemClickCommand; }
        }

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

        public ObservableCollection<LoginProviderViewModel> LoginProviders { get; set; }

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
                    return;

                if (createUserResult)
                    m_navigationService.Navigate(typeof(GroupListView));
            });
        }
    }
}