using System;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Clients;
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

        private const string NewMainServiceEndpointName = "MainService";
        private const string EncryptedEndpointName = "ItJakubServiceEncrypted";
        private const string MainServiceEndpointName = "ItJakubService";
        private const string MainServiceEndpointNameAuthenticated = "ItJakubService.Authenticated";
        private const string StreamedServiceEndpointName = "ItJakubServiceStreamed";
        private const string StreamedServiceEndpointNameAuthenticated = "ItJakubServiceStreamed.Authenticated";
        
        private const string LemmatizationServiceEndpointName = "LemmatizationService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider, IHttpContextAccessor httpContextAccessor)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_httpContextAccessor = httpContextAccessor;
        }

        public MainServiceRestClient GetMainServiceClient()
        {
            var uri = m_configurationProvider.GetEndpointUri(NewMainServiceEndpointName);
            var authToken = m_httpContextAccessor.HttpContext.GetTokenAsync(AuthenticationManager.AuthenticationTokenName)
                .GetAwaiter().GetResult();
            return new MainServiceRestClient(uri, authToken);
        }

        public IItJakubService GetAuthenticatedClient(string username, string commToken)
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(MainServiceEndpointNameAuthenticated);
            var binding = m_configurationProvider.GetBasicHttpsBindingUserNameAuthentication();
            var client = new ItJakubServiceClient(binding, endpoint);
            if (client.ClientCredentials == null)
            {
                throw new ArgumentException("Cannot set credentials for client");
            }

            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = commToken;
            return client;
        }

        public IItJakubService GetUnsecuredClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(MainServiceEndpointName);
            var binding = m_configurationProvider.GetBasicHttpBinding();
            var client = new ItJakubServiceClient(binding, endpoint);
            return client;
        }
        
        public LemmatizationServiceClient GetLemmatizationClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(LemmatizationServiceEndpointName);
            var binding = new BasicHttpBinding();
            return new LemmatizationServiceClient(binding, endpoint);
        }
    }
}