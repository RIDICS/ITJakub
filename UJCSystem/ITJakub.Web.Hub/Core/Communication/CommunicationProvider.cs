using System.ServiceModel;
using ITJakub.Lemmatization.Shared.Contracts;
using Vokabular.MainService.DataContracts.Clients;

namespace ITJakub.Web.Hub.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly MainServiceRestClient m_mainServiceRestClient;
        private readonly MainServiceBookClient m_bookClient;
        private readonly MainServiceCategoryClient m_categoryClient;
        private readonly MainServiceMetadataClient m_metadataClient;
        private readonly MainServiceProjectClient m_projectClient;
        private readonly MainServiceResourceClient m_resourceClient;
        private readonly MainServiceRoleClient m_roleClient;
        private readonly MainServiceUserClient m_userClient;

        private const string LemmatizationServiceEndpointName = "LemmatizationService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider,
            MainServiceRestClient mainServiceRestClient, MainServiceBookClient bookClient, MainServiceCategoryClient categoryClient,
            MainServiceMetadataClient metadataClient, MainServiceProjectClient projectClient, MainServiceResourceClient resourceClient,
            MainServiceRoleClient roleClient, MainServiceUserClient userClient)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_mainServiceRestClient = mainServiceRestClient;
            m_bookClient = bookClient;
            m_categoryClient = categoryClient;
            m_metadataClient = metadataClient;
            m_projectClient = projectClient;
            m_resourceClient = resourceClient;
            m_roleClient = roleClient;
            m_userClient = userClient;
        }

        public MainServiceRestClient GetMainServiceClient()
        {
            return m_mainServiceRestClient;
        }

        public MainServiceBookClient GetMainServiceBookClient()
        {
            return m_bookClient;
        }

        public MainServiceCategoryClient GetMainServiceCategoryClient()
        {
            return m_categoryClient;
        }

        public MainServiceMetadataClient GetMainServiceMetadataClient()
        {
            return m_metadataClient;
        }

        public MainServiceProjectClient GetMainServiceProjectClient()
        {
            return m_projectClient;
        }

        public MainServiceResourceClient GetMainServiceResourceClient()
        {
            return m_resourceClient;
        }

        public MainServiceRoleClient GetMainServiceRoleClient()
        {
            return m_roleClient;
        }

        public MainServiceUserClient GetMainServiceUserClient()
        {
            return m_userClient;
        }

        public LemmatizationServiceClient GetLemmatizationClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(LemmatizationServiceEndpointName);
            var binding = new BasicHttpBinding();
            return new LemmatizationServiceClient(binding, endpoint);
        }
    }
}