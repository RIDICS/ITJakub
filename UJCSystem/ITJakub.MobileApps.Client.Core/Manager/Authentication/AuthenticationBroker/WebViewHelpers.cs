using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker
{
    public class WebViewHelpers
    {
        public static readonly DependencyProperty BrowserTitleProperty = DependencyProperty.RegisterAttached("BrowserTitle", typeof(string), typeof(WebViewHelpers), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty NavigationCompletedCommandProperty = DependencyProperty.RegisterAttached("NavigationCompletedCommand", typeof(ICommand), typeof(WebViewHelpers), new PropertyMetadata(null));
        public static readonly DependencyProperty NavigationStartingCommandProperty = DependencyProperty.RegisterAttached("NavigationStartingCommand", typeof(ICommand), typeof(WebViewHelpers), new PropertyMetadata(null));

        public static string GetBrowserTitle(WebView element)
        {
            return (string)element.GetValue(BrowserTitleProperty);
        }

        public static void SetBrowserTitle(WebView element, string value)
        {
            element.SetValue(BrowserTitleProperty, value);
        }

        public static ICommand GetNavigationCompletedCommand(WebView element)
        {
            return (ICommand) element.GetValue(NavigationCompletedCommandProperty);
        }

        public static void SetNavigationCompletedCommand(WebView element, ICommand value)
        {
            element.SetValue(NavigationCompletedCommandProperty, value);
        }

        public static ICommand GetNavigationStartingCommand(WebView element)
        {
            return (ICommand) element.GetValue(NavigationStartingCommandProperty);
        }

        public static void SetNavigationStartingCommand(WebView element, ICommand value)
        {
            element.SetValue(NavigationStartingCommandProperty, value);
        }
    }
}
