using ITJakub.FileProcessing.DataContracts;
using Vokabular.FulltextService.DataContracts.Clients;

namespace Vokabular.MainService.Core.Communication
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
    }
}