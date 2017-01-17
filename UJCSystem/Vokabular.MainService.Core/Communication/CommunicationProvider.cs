using ITJakub.FileProcessing.DataContracts;

namespace Vokabular.MainService.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;

        private const string FileProcessingServiceEndpointName = "FileProcessingService";
        
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
    }
}