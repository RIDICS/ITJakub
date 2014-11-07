using System;

namespace ITJakub.DataEntities.Database.Entities
{
    public class BookBibl : IEquatable<BookBibl>
    {
        public virtual int Id { get; set; }
        public virtual BookVersion BookVersion { get; set; }
        public virtual string Text { get; set; }
        public virtual string Type { get; set; }
        public virtual string SubType { get; set; }
        public virtual short? BiblType { get; set; } //TODO enum

        public virtual bool Equals(BookBibl other)
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
            return Equals((BookBibl) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}