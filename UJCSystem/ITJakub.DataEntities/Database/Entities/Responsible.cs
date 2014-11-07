using System;
using System.Collections.Generic;

namespace ITJakub.DataEntities.Database.Entities
{
    public class Responsible : IEquatable<Responsible>
    {
        public virtual int Id { get; set; }
        public virtual ResponsibleType ResponsibleType { get; set; }
        public virtual string Text { get; set; }
        public virtual IList<BookVersion> BookVersions { get; set; }

        public virtual bool Equals(Responsible other)
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
            return Equals((Responsible) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}