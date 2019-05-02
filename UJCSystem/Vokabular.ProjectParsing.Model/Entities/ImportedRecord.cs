using System;

namespace Vokabular.ProjectParsing.Model.Entities
{
    public class ImportedRecord
    {
        public string ExternalId { get; set; }

        public object RawData { get; set; }

        public bool IsNew { get; set; }

        public bool IsSuitable { get; set; }

        public bool IsFailed { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string FaultedMessage { get; set; }

        public ImportedProject ImportedProject { get; set; }

        public long ProjectId { get; set; }

        public int? ImportedProjectMetadataId { get; set; }
    }
}