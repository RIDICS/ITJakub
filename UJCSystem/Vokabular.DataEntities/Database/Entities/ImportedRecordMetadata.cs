using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ImportedRecordMetadata : IEquatable<ImportedRecordMetadata>
    {
        public virtual int Id { get; set; }

        public virtual string LastUpdateMessage { get; set; }

        public virtual ImportHistory LastUpdate { get; set; }

        public virtual ImportedProjectMetadata ImportedProjectMetadata { get; set; }

        public virtual Snapshot Snapshot { get; set; }

        public virtual bool Equals(ImportedRecordMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ImportedRecordMetadata) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}