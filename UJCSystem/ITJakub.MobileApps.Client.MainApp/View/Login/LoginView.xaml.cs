// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

using Windows.UI.Xaml.Navigation;
<<<<<<< HEAD:UJCSystem/ITJakub.MobileApps.Client.MainApp/View/LoginView.xaml.cs
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
=======
>>>>>>> 76f07b70317554fb477fd5225878b9cf1ddc05ba:UJCSystem/ITJakub.MobileApps.Client.MainApp/View/Login/LoginView.xaml.cs
using ITJakub.MobileApps.Client.MainApp.Common;

namespace ITJakub.MobileApps.Client.MainApp.View.Login
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
            navigationHelper = new NavigationHelper(this);
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