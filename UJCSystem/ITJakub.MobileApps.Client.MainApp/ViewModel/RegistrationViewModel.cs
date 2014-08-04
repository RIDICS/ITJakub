using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
<<<<<<< HEAD
=======
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
>>>>>>> 76f07b70317554fb477fd5225878b9cf1ddc05ba
using ITJakub.MobileApps.Client.MainApp.View;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
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

        private void ItemClick(ItemClickEventArgs args)
        {
            var item = args.ClickedItem as LoginProviderViewModel;
            if (item == null)
                return;

            Register(item.LoginProviderType);
        }

        private void Register(LoginProviderType loginProviderType)
        {
            RegistrationInProgress = true;
            m_dataService.CreateUser(loginProviderType, (info, exception) =>
            {
                RegistrationInProgress = false;
                if (exception != null)
                    return;
                if (info.Success)
                    m_navigationService.Navigate(typeof(GroupListView));
            });
        }
    }
}