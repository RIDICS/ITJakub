// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker
{    
    public sealed partial class WebAuthView : Page
    {
        public WebAuthView(WebAuthViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void wv_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ((WebAuthViewModel) DataContext).BrowserTitle = Wv.DocumentTitle;//HACK for BrowserTitle cannot be bind
            ((WebAuthViewModel)DataContext).LoadAddresCompletedCommand.Execute(e);
        }

        private void wv_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            ((WebAuthViewModel)DataContext).BrowserTitle = Wv.DocumentTitle;//HACK for BrowserTitle cannot be bind
            ((WebAuthViewModel)DataContext).NavigationFailedCommand.Execute(e);
        }
    }
}