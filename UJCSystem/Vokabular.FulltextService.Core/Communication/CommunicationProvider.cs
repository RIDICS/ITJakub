using Vokabular.FulltextService.DataContracts.Clients;

namespace Vokabular.FulltextService.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;

        private const string FileProcessingServiceEndpointName = "FileProcessingService";
        private const string FulltextServiceEndpointName = "FulltextService";
        
        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider)
        {
            m_configurationProvider = communicationConfigurationProvider;
        }

        // TODO get client for Elasticsearch
    }
}