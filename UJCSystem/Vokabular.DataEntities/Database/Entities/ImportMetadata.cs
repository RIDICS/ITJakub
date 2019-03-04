using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ImportMetadata : IEquatable<ImportMetadata>
    {
        public virtual int Id { get; set; }

        public virtual string ExternalId { get; set; }

        public virtual string LastUpdateMessage { get; set; }

        public virtual ImportHistory LastUpdate { get; set; }

        public virtual ImportHistory LastSuccessfulUpdate { get; set; }

        public virtual Snapshot Snapshot { get; set; }

        public virtual bool Equals(ImportMetadata other)
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
            return Equals((ImportMetadata) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}