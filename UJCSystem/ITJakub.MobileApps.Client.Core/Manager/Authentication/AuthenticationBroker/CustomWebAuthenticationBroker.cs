using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker.Messages;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker
{
    public sealed class CustomWebAuthenticationBroker : IDisposable
    {
        private const int Width = 800;
        private readonly TaskCompletionSource<bool> m_taskCompletion;
        private Popup m_authenticationPopup;
        private bool m_disposed;

        private string m_responseData;
        private WebAuthenticationStatus m_responseStatus;
        private WebAuthView m_webAuthView;

        private CustomWebAuthenticationBroker(WebAuthenticationOptions options, Uri startUri, Uri endUri)
        {
            Options = options;
            StartUri = startUri;
            EndUri = endUri;

            RegisterForMessages();
            m_taskCompletion = new TaskCompletionSource<bool>();
        }

        private WebAuthenticationOptions Options { get; set; }
        private Uri StartUri { get; set; }
        private Uri EndUri { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void RegisterForMessages()
        {
            Messenger.Default.Register<AuthBrokerCanceledMessage>(this, WebAuthCanceled);
            Messenger.Default.Register<AuthBrokerUriNavigationFailedMessage>(this, UriNavigationFailed);
            Messenger.Default.Register<AuthBrokerUriChangedMessage>(this, UriNavigationChanged);
        }

        private void UriNavigationChanged(AuthBrokerUriChangedMessage message)
        {
            if (message.Uri.AbsoluteUri.StartsWith(EndUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
            {
                m_responseStatus = WebAuthenticationStatus.Success;
                m_responseData = GetResponseData(message);
                m_taskCompletion.TrySetResult(true);
                m_authenticationPopup.IsOpen = false;
            }
        }

        private void UriNavigationFailed(AuthBrokerUriNavigationFailedMessage message)
        {
            if (message.Uri.AbsoluteUri.StartsWith(EndUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
            {
                m_responseStatus = WebAuthenticationStatus.Success;
                m_responseData = GetResponseData(message);
            }
            else
            {
                m_responseStatus = WebAuthenticationStatus.ErrorHttp;
            }

            m_taskCompletion.TrySetResult(true);
            m_authenticationPopup.IsOpen = false;
        }

        private void WebAuthCanceled(AuthBrokerCanceledMessage message)
        {
            m_responseStatus = WebAuthenticationStatus.UserCancel;
            m_taskCompletion.TrySetResult(false);
            m_authenticationPopup.IsOpen = false;
        }

        private string GetResponseData(AuthBrokerUriChangedMessage message)
        {
            switch (Options)
            {
                case WebAuthenticationOptions.None:
                    return message.Uri.AbsoluteUri;
                case WebAuthenticationOptions.UseTitle:
                    return message.DocumentTitle;
                default:
                    throw new NotSupportedException(string.Format("{0} is not supported option for custom auth broker", Options));
            }
        }

        private async Task<AuthBrokerResult> Authenticate()
        {
            using (var viewModel = new WebAuthViewModel())
            {
                m_authenticationPopup = new Popup
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Width = Width,
                    Height = Window.Current.Bounds.Height
                };
                InputPane.GetForCurrentView().Showing += KeyboardShowing;
                InputPane.GetForCurrentView().Hiding += KeyboardHiding;

                m_webAuthView = new WebAuthView(viewModel)
                {
                    Width = Window.Current.Bounds.Width,
                    Height = Window.Current.Bounds.Height
                };

                m_authenticationPopup.Child = m_webAuthView;
                m_authenticationPopup.IsOpen = true;
                m_responseStatus = WebAuthenticationStatus.Success;
                m_responseData = string.Empty;

                Messenger.Default.Send(new AuthBrokerNavigateUriMessage {Uri = StartUri, Options = Options});
            }

            await m_taskCompletion.Task;

            return new AuthBrokerResult {ResponseStatus = m_responseStatus, ResponseData = m_responseData};
        }

        public static async Task<AuthBrokerResult> AuthenticateAsync(WebAuthenticationOptions options, Uri startUri, Uri endUri)
        {
            using (var authBroker = new CustomWebAuthenticationBroker(options, startUri, endUri))
            {
                return await authBroker.Authenticate();
            }
        }

        private void KeyboardHiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            m_authenticationPopup.VerticalOffset = 0;
            m_webAuthView.Height = Window.Current.Bounds.Height;
        }

        private void KeyboardShowing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            m_authenticationPopup.VerticalOffset -= 100;
            m_webAuthView.Height = m_webAuthView.Height - args.OccludedRect.Height + 200;
        }
      


        private void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            if (disposing)
            {
                UnregisterFromMessages();
                InputPane.GetForCurrentView().Hiding -= KeyboardHiding;
                InputPane.GetForCurrentView().Showing -= KeyboardShowing;
            }
            m_disposed = true;
        }

        private void UnregisterFromMessages()
        {
            Messenger.Default.Unregister(this);
        }
    }
}