using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ProjectImportMetadata : IEquatable<ProjectImportMetadata>
    {
        public virtual int Id { get; set; }

        public virtual string ExternalId { get; set; }

        public virtual string LastUpdateMessage { get; set; }

        public virtual ImportHistory LastUpdate { get; set; }

        public virtual ImportHistory LastSuccessfulUpdate { get; set; }

        public virtual Project Project { get; set; }

        public virtual ExternalResource ExternalResource { get; set; }

        public virtual bool Equals(ProjectImportMetadata other)
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
            return Equals((ProjectImportMetadata) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}