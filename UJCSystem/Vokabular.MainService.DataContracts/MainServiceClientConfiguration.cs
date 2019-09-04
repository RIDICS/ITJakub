using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient;

namespace Vokabular.MainService.DataContracts
{
    public class MainServiceClientConfiguration : ServiceCommunicationConfiguration
    {
        public PortalTypeContract PortalType { get; set; }
    }
}