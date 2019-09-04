using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.DataEntities.Database.Entities.SelectResults
{
    public class ProjectIdentificationResult
    {
        public long ProjectId { get; set; }

        public string ProjectExternalId { get; set; }

        public long SnapshotId { get; set; }

        public string BookVersionExternalId { get; set; }

        public ProjectTypeEnum ProjectType { get; set; }
    }
}