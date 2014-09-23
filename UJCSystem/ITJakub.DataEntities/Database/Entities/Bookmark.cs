using System;

namespace ITJakub.DataEntities.Database.Entities
{
    public class Bookmark : IEquatable<Bookmark>
    {
        public virtual long Id { get; set; }
        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
        public virtual string Page { get; set; }

        public bool Equals(Bookmark other)
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
            return Equals((Bookmark) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}