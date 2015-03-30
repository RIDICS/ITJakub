using System;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker.Messages;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker
{
    public class WebAuthViewModel : ViewModelBase, IDisposable
    {
        private string m_browserTitle;
        private Uri m_browserUri;
        private bool m_disposed;
        private bool m_isError;
        private bool m_loading;

        public WebAuthViewModel()
        {
            InitializeCommands();
            RegisterForMessages();
        }


        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand NavigationStartingCommand { get; private set; }
        public RelayCommand<WebViewNavigationCompletedEventArgs> NavigationCompletedCommand { get; private set; } 

        public string BrowserTitle
        {
            get { return m_browserTitle; }
            set{m_browserTitle = value;RaisePropertyChanged();}
        } 

        public Uri BrowserUri
        {
            get { return m_browserUri; }
            set{m_browserUri = value;RaisePropertyChanged();}
        }

        public bool IsError
        {
            get { return m_isError; }
            set
            {
                m_isError = value; 
                RaisePropertyChanged();
            }
        }

        public bool Loading
        {
            get { return m_loading; }
            set
            {
                m_loading = value;
                RaisePropertyChanged();
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void InitializeCommands()
        {
            CancelCommand = new RelayCommand(() => MessengerInstance.Send(new AuthBrokerCanceledMessage()));

            NavigationStartingCommand = new RelayCommand(() => Loading = true);

            NavigationCompletedCommand = new RelayCommand<WebViewNavigationCompletedEventArgs>(parameters =>
            {
                Loading = false;

                if (parameters.IsSuccess)
                    MessengerInstance.Send(new AuthBrokerUriChangedMessage {Uri = parameters.Uri, DocumentTitle = BrowserTitle});
                else
                    IsError = true;
            });
        }

        private void UnregisterFromMessages()
        {
            MessengerInstance.Unregister(this);
        }

        private void RegisterForMessages()
        {
            MessengerInstance.Register<AuthBrokerNavigateUriMessage>(this, message =>
            {
                ClearCookies(message.Uri);
                BrowserUri = message.Uri;
            });
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

        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            if (disposing)
            {
                UnregisterFromMessages();
            }

            m_disposed = true;
        }
    }
}