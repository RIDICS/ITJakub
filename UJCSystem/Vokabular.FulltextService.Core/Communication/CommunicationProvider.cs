using System;
using Nest;

namespace Vokabular.FulltextService.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly ElasticClient m_elasticClient;

        private const string ElasticSearchService = "ElasticSearchService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_elasticClient = CreateElasticClient(); // should be singleton
        }

        private ElasticClient CreateElasticClient()
        {
            var baseAddress = m_configurationProvider.GetEndpointUri(ElasticSearchService);
            var settings = new ConnectionSettings(baseAddress)
                .RequestTimeout(TimeSpan.FromMinutes(2));
            
                // If debug info required, uncomment following lines:
                //.DisableDirectStreaming()
                //.OnRequestCompleted(details =>
                //{
                //    Debug.WriteLine("### REQUEST ###");
                //    if (details.RequestBodyInBytes != null) Debug.WriteLine(Encoding.UTF8.GetString(details.RequestBodyInBytes));
                //    Debug.WriteLine("### RESPONSE ###");
                //    if (details.ResponseBodyInBytes != null) Debug.WriteLine(Encoding.UTF8.GetString(details.ResponseBodyInBytes));
                //})
                //.PrettyJson();

            var client = new ElasticClient(settings);
            return client;
        }

        public ElasticClient GetElasticClient()
        {
            return m_elasticClient;
        }
    }
}