using System.ServiceModel;
using ITJakub.Lemmatization.Shared.Contracts;
using Vokabular.MainService.DataContracts.Clients;

namespace ITJakub.Web.Hub.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly MainServiceRestClient m_mainServiceRestClient;
        private readonly MainServiceProjectClient m_projectClient;
        private readonly MainServiceMetadataClient m_metadataClient;

        private const string LemmatizationServiceEndpointName = "LemmatizationService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider,
            MainServiceRestClient mainServiceRestClient, MainServiceProjectClient projectClient, MainServiceMetadataClient metadataClient)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_mainServiceRestClient = mainServiceRestClient;
            m_projectClient = projectClient;
            m_metadataClient = metadataClient;
        }

        public MainServiceRestClient GetMainServiceClient()
        {
            return m_mainServiceRestClient;
        }

        public MainServiceProjectClient GetMainServiceProjectClient()
        {
            return m_projectClient;
        }

        public MainServiceMetadataClient GetMainServiceMetadataClient()
        {
            return m_metadataClient;
        }

        public LemmatizationServiceClient GetLemmatizationClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(LemmatizationServiceEndpointName);
            var binding = new BasicHttpBinding();
            return new LemmatizationServiceClient(binding, endpoint);
        }
    }
}