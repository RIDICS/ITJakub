using System;
using System.Collections.Generic;

namespace ITJakub.DataEntities.Database.Entities
{
    public class BookVersion : IEquatable<BookVersion>
    {
        public virtual long Id { get; set; }
        public virtual Book Book { get; set; }
        public virtual string Guid { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<Author> Authors { get; set; }

        public bool Equals(BookVersion other)
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
            return Equals((BookVersion) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}