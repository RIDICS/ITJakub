using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class OriginalAuthor : IEquatable<OriginalAuthor>
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual IList<ProjectOriginalAuthor> Projects { get; set; }

        public virtual bool Equals(OriginalAuthor other)
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
            return Equals((OriginalAuthor) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}