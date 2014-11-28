using System;

namespace ITJakub.DataEntities.Database.Entities
{
    public class BookPage : IEquatable<BookPage>
    {
        public virtual long Id { get; set; }
        public virtual BookVersion BookVersion { get; set; }
        public virtual string Text { get; set; }
        public virtual string XmlId { get; set; }
        public virtual int Position { get; set; }

        public virtual bool Equals(BookPage other)
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
            return Equals((BookPage) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}