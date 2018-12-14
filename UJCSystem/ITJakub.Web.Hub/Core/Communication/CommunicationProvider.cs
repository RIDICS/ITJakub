using System.ServiceModel;
using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Core.Managers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Vokabular.MainService.DataContracts.Clients;

namespace ITJakub.Web.Hub.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly IHttpContextAccessor m_httpContextAccessor;

        private const string MainServiceEndpointName = "MainService";
        private const string LemmatizationServiceEndpointName = "LemmatizationService";
        private const string AuthenticationTokenName = "access_token";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider, IHttpContextAccessor httpContextAccessor)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_httpContextAccessor = httpContextAccessor;
        }

        private string GetCommunicationToken()
        {
            var communicationToken = m_httpContextAccessor.HttpContext.GetTokenAsync(AuthenticationTokenName)
                .GetAwaiter().GetResult();
            return communicationToken;
        }

        public MainServiceRestClient GetMainServiceClient()
        {
            var uri = m_configurationProvider.GetEndpointUri(MainServiceEndpointName);
            var authToken = GetCommunicationToken();
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