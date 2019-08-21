using Vokabular.RestClient;

namespace Vokabular.FulltextService.DataContracts.Clients
{
    public class FulltextServiceRestClient : FullRestClient
    {
        public FulltextServiceRestClient(FulltextServiceClientConfiguration communicationConfiguration) : base(communicationConfiguration)
        {
            
        }
    }
}
