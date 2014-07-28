using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.MainApp.View;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
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
        private readonly IDataService m_dataService;
        private readonly INavigationService m_navigationService;
        private readonly RelayCommand<ItemClickEventArgs> m_itemClickCommand;
        private Visibility m_loginDialogVisibility;
        private bool m_loggingIn;

        /// <summary>
        /// Initializes a new instance of the LoginViewModel class.
        /// </summary>
        public LoginViewModel(IDataService dataService, INavigationService navigationService)
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
            m_navigationService = navigationService;
            LoggingIn = false;
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

        private void ItemClick(ItemClickEventArgs args)
        {
            var item = args.ClickedItem as LoginMenuItemViewModel;
            if (item == null)
                return;

            LoggingIn = true;
            m_dataService.LoginAsync(item.LoginProvider, (info, exception) =>
            {
                LoggingIn = false;
                if (exception != null)
                    return;
                if (info.Success)
                    m_navigationService.Navigate(typeof(GroupListView));
            });
        }
    }
}