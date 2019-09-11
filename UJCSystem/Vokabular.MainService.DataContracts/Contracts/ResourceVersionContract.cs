using System;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ResourceVersionContract
    {
        public long Id { get; set; }
        public int VersionNumber { get; set; }
        public string Comment { get; set; }
        public DateTime CreateDate { get; set; }
        public string Author { get; set; }
    }

    public class ResourceVersionDetailContract : ResourceVersionContract
    {
        public long ResourceId { get; set; }
        public ResourceTypeEnumContract ResourceType { get; set; }
    }
}