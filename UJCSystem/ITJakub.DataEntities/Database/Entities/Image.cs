using System;

namespace ITJakub.DataEntities.Database.Entities
{
    public class Image : IEquatable<Image>
    {
        public virtual long Id { get; set; }
        public virtual Book Book { get; set; }
        public virtual string FileName { get; set; }
        public virtual short ImageType { get; set; }

        public bool Equals(Image other)
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
            return Equals((Image) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}