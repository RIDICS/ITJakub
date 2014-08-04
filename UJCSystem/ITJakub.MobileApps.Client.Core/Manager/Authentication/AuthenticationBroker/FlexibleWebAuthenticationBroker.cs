using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker
{
    public static class FlexibleWebAuthenticationBroker
    {
        public static async Task<FlexibleWebAuthenticationResult> AuthenticateAsync(WebAuthenticationOptions options, Uri startUri, Uri endUri)
        {
            var tcs = new TaskCompletionSource<int>();

            var responseStatus = WebAuthenticationStatus.Success;
            string responseData = "";

            var p = new Popup
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,

                Width = 800,
                Height = Window.Current.Bounds.Height
            };

            var f = new FlexibleWebAuthView
            {
                Width = Window.Current.Bounds.Width,
                Height = Window.Current.Bounds.Height
            };

            f.CancelledEvent += (s, e) =>
            {
                responseStatus = WebAuthenticationStatus.UserCancel;
                tcs.TrySetResult(1);
                p.IsOpen = false;
            };

            f.UriChangedEvent += (s, e) =>
            {
                if (((Uri) s).AbsoluteUri.StartsWith(endUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
                {
                    responseStatus = WebAuthenticationStatus.Success;
                    responseData = ((Uri) s).AbsoluteUri;
                    tcs.TrySetResult(1);
                    p.IsOpen = false;
                }
            };

            f.NavFailedEvent += (s, e) =>
            {
                if (((Uri) s).AbsoluteUri.StartsWith(endUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
                {
                    responseStatus = WebAuthenticationStatus.Success;
                    responseData = ((Uri) s).AbsoluteUri;
                }
                else
                {
                    responseStatus = WebAuthenticationStatus.ErrorHttp;
                }

                tcs.TrySetResult(1);
                p.IsOpen = false;

            };

            p.Child = f;
            p.IsOpen = true;
            f.Navigate(startUri);
            await tcs.Task;


            return new FlexibleWebAuthenticationResult {ResponseStatus = responseStatus, ResponseData = responseData};
        }
    }
}