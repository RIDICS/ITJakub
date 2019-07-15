using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class FilteringExpressionSet : IEquatable<FilteringExpressionSet>
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual BibliographicFormat BibliographicFormat { get; set; }

        public virtual IList<FilteringExpression> FilteringExpressions { get; set; }

        public virtual IList<ExternalRepository> ExternalRepositories { get; set; }

        public virtual bool Equals(FilteringExpressionSet other)
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
            return Equals((FilteringExpressionSet) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}