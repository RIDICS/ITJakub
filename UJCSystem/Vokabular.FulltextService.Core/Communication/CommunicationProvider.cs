using System;
using Nest;

namespace Vokabular.FulltextService.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;

        private const string FileProcessingServiceEndpointName = "FileProcessingService";
        private const string FulltextServiceEndpointName = "FulltextService";
        private const string ElasticSearchService = "ElasticSearchService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider)
        {
            m_configurationProvider = communicationConfigurationProvider;
        }

        // TODO get client for Elasticsearch
        public ElasticClient GetElasticClient()
        {
            var baseAdrress = m_configurationProvider.GetEndpointUri(ElasticSearchService);
            var settings = new ConnectionSettings(baseAdrress)
                .RequestTimeout(TimeSpan.FromMinutes(2));
            ElasticClient client = new ElasticClient(settings);

            return client;
        }
    }
}