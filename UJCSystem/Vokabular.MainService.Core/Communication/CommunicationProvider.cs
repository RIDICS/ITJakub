using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.DataContracts;
using ITJakub.SearchService.DataContracts;
using Microsoft.Extensions.Options;
using Vokabular.CardFile.Core;
using Vokabular.FulltextService.DataContracts.Clients;
using Vokabular.Shared.Options;

namespace Vokabular.MainService.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly IOptions<List<CredentialsOption>> m_credentialsOptions;

        private const string FileProcessingServiceEndpointName = "FileProcessingService";
        private const string FulltextServiceEndpointName = "FulltextService";
        private const string SearchServiceEndpointName = "SearchService";
        private const string CardFilesEndpointName = "CardFilesService";
        private const string CardFilesCredentials = "CardFiles";
        
        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider, IOptions<List<CredentialsOption>> credentialsOptions)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_credentialsOptions = credentialsOptions;
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
    }
}