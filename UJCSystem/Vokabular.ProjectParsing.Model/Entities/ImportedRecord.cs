namespace Vokabular.ProjectParsing.Model.Entities
{
    public class ImportedRecord
    {
        public string ExternalId { get; set; }

        public object RawData { get; set; }

        public bool IsNew { get; set; }

        public bool IsSuitable { get; set; }

        public bool IsFailed { get; set; }

        public string FaultedMessage { get; set; }

        public Project Project { get; set; }

        public long ProjectId { get; set; }

        public int? ImportedProjectMetadataId { get; set; }
    }
}