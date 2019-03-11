namespace Vokabular.ProjectParsing.Model.Entities
{
    public class ProjectImportMetadata
    {
        public string ExternalId { get; set; }

        public object RawData { get; set; }

        public bool IsNew { get; set; }

        public bool IsSuitable { get; set; }

        public bool IsFaulted { get; set; }

        public string FaultedMessage { get; set; }

        public Project Project { get; set; }

        public long ProjectId { get; set; }
    }
}