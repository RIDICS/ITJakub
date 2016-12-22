using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class Publisher : IEquatable<Publisher>
    {
        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public virtual string Email { get; set; }

        public virtual bool Equals(Publisher other)
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
            return Equals((Publisher) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}