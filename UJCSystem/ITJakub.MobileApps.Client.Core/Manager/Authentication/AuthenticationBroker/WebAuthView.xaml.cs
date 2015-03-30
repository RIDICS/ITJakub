// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker
{    
    public sealed partial class WebAuthView : Page
    {
        public WebAuthView(WebAuthViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Wv_OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            WebViewHelpers.SetBrowserTitle(sender, sender.DocumentTitle);
            
            var command = WebViewHelpers.GetNavigationCompletedCommand(sender);
            if (command != null)
                command.Execute(args);
        }

        private void Wv_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            var command = WebViewHelpers.GetNavigationStartingCommand(sender);
            if (command != null)
                command.Execute(args);
        }
    }
}