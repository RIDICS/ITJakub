using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ResourceContract
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ResourceTypeEnumContract ResourceType { get; set; }
    }

    public class ResourceWithLatestVersionContract : ResourceContract
    {
        public ResourceVersionWithRelatedInfoContract LatestVersion { get; set; }
    }

    public class RelatedResourceContract : ResourceContract
    {
        public int Sequence { get; set; }
    }
}