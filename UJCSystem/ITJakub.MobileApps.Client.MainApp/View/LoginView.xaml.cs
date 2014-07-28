// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.MainApp.Common;
using ITJakub.MobileApps.Client.MainApp.ViewModel;

namespace ITJakub.MobileApps.Client.MainApp.View
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class LoginView
    {
        private NavigationHelper navigationHelper;

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public LoginView()
        {
            InitializeComponent();
            InitMenu();
            navigationHelper = new NavigationHelper(this);
        }

        private void InitMenu()
        {
            MenuListView.ItemsSource = new ObservableCollection<LoginMenuItemViewModel>
            {
                new LoginMenuItemViewModel
                {
                    LoginProvider = LoginProvider.LiveId,
                    Icon = new BitmapImage(new Uri("ms-appx:///Icon/windows8-128.png")),
                    Name = "Live ID"
                },
                new LoginMenuItemViewModel
                {
                    LoginProvider = LoginProvider.Facebook,
                    Icon = new BitmapImage(new Uri("ms-appx:///Icon/facebook-128.png")),
                    Name = "Facebook"
                },
                new LoginMenuItemViewModel
                {
                    LoginProvider = LoginProvider.Google,
                    Icon = new BitmapImage(new Uri("ms-appx:///Icon/google_plus-128.png")),
                    Name = "Google"
                }
            };
        }


        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

    }
}