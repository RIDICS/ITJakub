using System.ServiceModel;
using ITJakub.Lemmatization.Shared.Contracts;
using Vokabular.MainService.DataContracts.Clients;

namespace ITJakub.Web.Hub.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly MainServiceRestClient m_mainServiceRestClient;
        private readonly MainServiceMetadataClient m_metadataClient;
        private readonly MainServiceProjectClient m_projectClient;
        private readonly MainServiceRoleClient m_roleClient;
        private readonly MainServiceUserClient m_userClient;


        private const string LemmatizationServiceEndpointName = "LemmatizationService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider,
            MainServiceRestClient mainServiceRestClient, MainServiceMetadataClient metadataClient, MainServiceProjectClient projectClient,
            MainServiceRoleClient roleClient, MainServiceUserClient userClient)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_mainServiceRestClient = mainServiceRestClient;
            m_metadataClient = metadataClient;
            m_projectClient = projectClient;
            m_roleClient = roleClient;
            m_userClient = userClient;
        }

        public MainServiceRestClient GetMainServiceClient()
        {
            return m_mainServiceRestClient;
        }

        public MainServiceMetadataClient GetMainServiceMetadataClient()
        {
            return m_metadataClient;
        }

        public MainServiceProjectClient GetMainServiceProjectClient()
        {
            return m_projectClient;
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