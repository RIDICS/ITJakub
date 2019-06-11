using System.ServiceModel;
using ITJakub.Lemmatization.Shared.Contracts;
using Vokabular.Authentication.Client.SharedClient.Provider;
using Vokabular.MainService.DataContracts.Clients;

namespace ITJakub.Web.Hub.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly AuthApiAccessTokenProvider m_authApiAccessTokenProvider;

        private const string MainServiceEndpointName = "MainService";
        private const string LemmatizationServiceEndpointName = "LemmatizationService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider, AuthApiAccessTokenProvider authApiAccessTokenProvider)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_authApiAccessTokenProvider = authApiAccessTokenProvider;
        }

        public MainServiceRestClient GetMainServiceClient()
        {
            var uri = m_configurationProvider.GetEndpointUri(MainServiceEndpointName);
            var authToken = m_authApiAccessTokenProvider.GetAccessTokenAsync().GetAwaiter().GetResult();
            return new MainServiceRestClient(uri, authToken);
        }
        
        public LemmatizationServiceClient GetLemmatizationClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(LemmatizationServiceEndpointName);
            var binding = new BasicHttpBinding();
            return new LemmatizationServiceClient(binding, endpoint);
        }
    }
}