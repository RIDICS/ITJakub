using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.DataEntities.Database.Entities.SelectResults
{
    public class SnapshotAggregatedInfo
    {
        public long Id { get; set; }
        public int ResourcesCount { get; set; }
        public ResourceTypeEnum Type { get; set; }
    }
}