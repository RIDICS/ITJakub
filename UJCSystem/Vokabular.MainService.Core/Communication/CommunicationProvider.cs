using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.DataContracts;
using ITJakub.SearchService.DataContracts;
using Microsoft.Extensions.Options;
using Vokabular.Authentication.Client;
using Vokabular.CardFile.Core;
using Vokabular.FulltextService.DataContracts.Clients;
using Vokabular.Shared.Options;

namespace Vokabular.MainService.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly IOptions<List<CredentialsOption>> m_credentialsOptions;
        private readonly IOptions<AuthServiceOption> m_authServiceApiKey;

        private const string FileProcessingServiceEndpointName = "FileProcessingService";
        private const string FulltextServiceEndpointName = "FulltextService";
        private const string SearchServiceEndpointName = "SearchService";
        private const string AuthenticationEndpointName = "AuthenticationService";
        private const string CardFilesEndpointName = "CardFilesService";
        private const string CardFilesCredentials = "CardFiles";
        
        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider, IOptions<List<CredentialsOption>> credentialsOptions, IOptions<AuthServiceOption> authServiceApiKey)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_credentialsOptions = credentialsOptions;
            m_authServiceApiKey = authServiceApiKey;
        }

        public FileProcessingServiceClient GetFileProcessingClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(FileProcessingServiceEndpointName);
            var binding = m_configurationProvider.GetBasicHttpBindingStreamed();
            var client = new FileProcessingServiceClient(binding, endpoint);
            return client;
        }

        public FulltextServiceClient GetFulltextServiceClient()
        {
            var uri = m_configurationProvider.GetEndpointUri(FulltextServiceEndpointName);
            return new FulltextServiceClient(uri);
        }

        public SearchServiceClient GetSearchServiceClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(SearchServiceEndpointName);
            var binding = m_configurationProvider.GetBasicHttpBinding();
            var client = new SearchServiceClient(binding, endpoint);
            return client;
        }

        public CardFilesClient GetCardFilesClient()
        {
            var uri = m_configurationProvider.GetEndpointUri(CardFilesEndpointName);
            var credentials = m_credentialsOptions.Value.FirstOrDefault(x => x.Type == CardFilesCredentials);
            if (credentials == null)
            {
                throw new ArgumentException("Credentials for Card files not found");
            }

            var client = new CardFilesClient(uri, credentials.Username, credentials.Password);
            return client;
        }

        public AuthenticationClient GetAuthenticationServiceClient()
        {
            throw new InvalidOperationException("This method will no longer be used. It will be replaced by obtaining data Client from IoC.");
            //var uri = m_configurationProvider.GetEndpointUri(AuthenticationEndpointName);
            //var credentials = m_authServiceApiKey.Value;
            //if (credentials?.Name == null || credentials.ApiKeyHash == null)
            //{
            //    throw new ArgumentException("Credentials for Authentication service not found");
            //}

            //var client = new AuthenticationClient(uri, credentials.Name, credentials.ApiKeyHash);
            //return client;
        }
    }
}