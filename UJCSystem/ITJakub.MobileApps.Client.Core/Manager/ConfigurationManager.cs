using System.Threading.Tasks;
using Windows.Storage;
using ITJakub.MobileApps.Client.Books;
using ITJakub.MobileApps.Client.Core.Communication.Client;
using ITJakub.MobileApps.Client.Core.Manager.News;

namespace ITJakub.MobileApps.Client.Core.Manager
{
    public class ConfigurationManager
    {
        private readonly MobileAppsServiceClientManager m_serviceClient;
        private readonly NewsManager m_newsManager;
        private readonly ApplicationDataContainer m_localSettings;
        private bool m_isBookLibraryAddressUpdated;

        public ConfigurationManager(MobileAppsServiceClientManager serviceClient, NewsManager newsManager)
        {
            m_serviceClient = serviceClient;
            m_newsManager = newsManager;
            m_localSettings = ApplicationData.Current.LocalSettings;
            m_isBookLibraryAddressUpdated = false;

            Init();
        }

        private void Init()
        {
            var endpointAddress = EndpointAddress;

            if (!string.IsNullOrWhiteSpace(endpointAddress))
                m_serviceClient.UpdateEndpointAddress(endpointAddress);
        }

        public string EndpointAddress
        {
            get { return m_localSettings.Values["EndpointAddress"] as string; }
            set
            {
                m_localSettings.Values["EndpointAddress"] = value;
                m_serviceClient.UpdateEndpointAddress(value);
            }
        }

        public async Task UpdateBookLibraryEndpointAddress()
        {
            if (m_isBookLibraryAddressUpdated)
                return;

            var client = m_serviceClient.GetClient();
            var address = await client.GetBookLibraryEndpointAddressAsync();
            Book.UpdateEndpointAddress(address);
            m_newsManager.NewsEndpoint = address;
            m_isBookLibraryAddressUpdated = true;
        }
    }
}
