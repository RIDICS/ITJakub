// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlexibleWebAuthView : Page
    {
        public FlexibleWebAuthView()
        {
            InitializeComponent();

            Loaded += FlexWebAuth_Loaded;

            wv.LoadCompleted += wv_LoadCompleted;
            wv.NavigationFailed += wv_NavigationFailed;
        }

        public EventHandler CancelledEvent { get; set; }
        public EventHandler UriChangedEvent { get; set; }
        public EventHandler NavFailedEvent { get; set; }

        private void wv_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            if (NavFailedEvent != null)
                NavFailedEvent.Invoke(e.Uri, null);
        }

        private void wv_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (UriChangedEvent != null)
                UriChangedEvent.Invoke(e.Uri, null);
        }

        private void FlexWebAuth_Loaded(object sender, RoutedEventArgs e)
        {
            wv.Width = 800;
        }


        public void Navigate(Uri uri)
        {
            ClearCookies(uri);
            wv.Navigate(uri);
        }

        private void ClearCookies(Uri uri)
        {
            var myFilter = new HttpBaseProtocolFilter();
            HttpCookieManager cookieManager = myFilter.CookieManager;
            HttpCookieCollection myCookieJar = cookieManager.GetCookies(uri);
            foreach (HttpCookie cookie in myCookieJar)
            {
                cookieManager.DeleteCookie(cookie);
            }
        }


        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            if (CancelledEvent != null)
                CancelledEvent.Invoke(null, null);
        }
    }
}