using System;
using Jewelery;

namespace ITJakub.DataEntities.Database.Entities
{
    public class BookAccessory : IEquatable<BookAccessory>
    {
        public virtual long Id { get; set; }

        public virtual BookVersion BookVersion { get; set; }
        
        public virtual AccessoryType Type { get; set; }

        public virtual string FileName { get; set; }

        public virtual bool Equals(BookAccessory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BookAccessory) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public enum AccessoryType : byte
    {
        Unknown=0,

        Cover = 1,

        Content = 2,

        Bibliography = 3,
    }
}