using System;
using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class SnapshotAggregatedInfoContract
    {
        public long Id { get; set; }
        public DateTime PublishDate { get; set; }
        public List<SnapshotResourcesInfoContract> ResourcesInfo { get; set; }
        public string Author { get; set; }
    }

    public class SnapshotResourcesInfoContract
    {
        public ResourceTypeEnumContract ResourceType { get; set; }
        public int PublishedCount { get; set; }
    }
}
