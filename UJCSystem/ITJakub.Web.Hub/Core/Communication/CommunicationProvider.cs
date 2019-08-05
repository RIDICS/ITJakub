using System.ServiceModel;
using ITJakub.Lemmatization.Shared.Contracts;
using Ridics.Core.HttpClient.Provider;
using Vokabular.MainService.DataContracts.Clients;

namespace ITJakub.Web.Hub.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly AuthApiAccessTokenProvider m_authApiAccessTokenProvider;
        private readonly MainServiceRestClient m_mainServiceRestClient;

        private const string LemmatizationServiceEndpointName = "LemmatizationService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider,
            AuthApiAccessTokenProvider authApiAccessTokenProvider, MainServiceRestClient mainServiceRestClient)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_authApiAccessTokenProvider = authApiAccessTokenProvider;
            m_mainServiceRestClient = mainServiceRestClient;
        }

        public MainServiceRestClient GetMainServiceClient()
        {
            var authToken = m_authApiAccessTokenProvider.GetAccessTokenAsync().GetAwaiter().GetResult();
            return m_mainServiceRestClient;
        }

        public LemmatizationServiceClient GetLemmatizationClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(LemmatizationServiceEndpointName);
            var binding = new BasicHttpBinding();
            return new LemmatizationServiceClient(binding, endpoint);
        }
    }
}