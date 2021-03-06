using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ExternalRepository : IEquatable<ExternalRepository>
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string Url { get; set; }

        public virtual string License { get; set; }

        public virtual string Configuration { get; set; }

        public virtual string UrlTemplate { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual BibliographicFormat BibliographicFormat { get; set; }

        public virtual ExternalRepositoryType ExternalRepositoryType { get; set; }

        public virtual IList<ImportHistory> ImportHistories { get; set; }

        public virtual IList<ImportedProjectMetadata> ImportedProjectMetadata { get; set; }

        public virtual IList<FilteringExpressionSet> FilteringExpressionSets { get; set; }

        public virtual bool Equals(ExternalRepository other)
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
            return Equals((ExternalRepository) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}