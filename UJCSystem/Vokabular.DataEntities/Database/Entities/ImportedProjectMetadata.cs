using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ImportedProjectMetadata : IEquatable<ImportedProjectMetadata>
    {
        public virtual int Id { get; set; }

        public virtual string ExternalId { get; set; }

        public virtual ExternalRepository ExternalRepository { get; set; }

        public virtual Project Project { get; set; }

        public virtual bool Equals(ImportedProjectMetadata other)
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
            return Equals((ImportedProjectMetadata) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}