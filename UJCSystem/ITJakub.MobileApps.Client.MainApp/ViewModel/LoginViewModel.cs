using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Core.Manager;
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
        private ObservableCollection<LoginItem> m_menuItems;
        private RelayCommand<ItemClickEventArgs> m_itemClickCommand;

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
            m_itemClickCommand = new RelayCommand<ItemClickEventArgs>(ItemClick);

            InitMenu();
        }

        private void InitMenu()
        {
            MenuItems = new ObservableCollection<LoginItem>
            {
                new LoginItem
                {
                    LoginProvider = LoginProvider.LiveId,
                    Icon = new BitmapImage(new Uri("ms-appx:///Icon/windows8-128.png")),
                    Name = "Live ID"
                },
                new LoginItem
                {
                    LoginProvider = LoginProvider.Facebook,
                    Icon = new BitmapImage(new Uri("ms-appx:///Icon/facebook-128.png")),
                    Name = "Facebook"
                },
                new LoginItem
                {
                    LoginProvider = LoginProvider.Google,
                    Icon = new BitmapImage(new Uri("ms-appx:///Icon/google_plus-128.png")),
                    Name = "Google"
                }
            };
        }

        public string Message { get; set; }

        public ObservableCollection<LoginItem> MenuItems
        {
            get { return m_menuItems; }
            set
            {
                m_menuItems = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<ItemClickEventArgs> ItemClickCommand
        {
            get { return m_itemClickCommand; }
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

            m_navigationService.Navigate(typeof(ApplicationSelection));
            //m_navigationService.Navigate(typeof(GroupListView));            
        }

        private void ItemClick(ItemClickEventArgs args)
        {
            var item = args.ClickedItem as LoginItem;
            if (item == null)
                return;

            switch (item.LoginProvider)
            {
                case LoginProvider.LiveId:
                    LiveIdLogin();
                    break;
                case LoginProvider.Facebook:
                    FacebookLogin();
                    break;
                case LoginProvider.Google:
                    GoogleLogin();
                    break;
            }
        }
    }

    public class LoginItem
    {
        public string Name { get; set; }
        public BitmapImage Icon { get; set; }
        public LoginProvider LoginProvider { get; set; }
    }
}