using System;

namespace ITJakub.DataEntities.Entities
{
    public class AuthorInfo : IEquatable<AuthorInfo>
    {
        public virtual int Id { get; set; }
        public virtual Author Author { get; set; }
        public virtual string Text { get; set; }
        public virtual int TextType { get; set; }

        public bool Equals(AuthorInfo other)
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
            return Equals((AuthorInfo) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}