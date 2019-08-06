using Microsoft.Extensions.Logging;
using Vokabular.RestClient;
using Vokabular.Shared;

namespace Vokabular.FulltextService.DataContracts.Clients
{
    public class FulltextServiceRestClient : FullRestClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<FulltextServiceClient>();
        
        public FulltextServiceRestClient(ServiceCommunicationConfiguration communicationConfiguration) : base(communicationConfiguration)
        {
            
        }
    }
}
