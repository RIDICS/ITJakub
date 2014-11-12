using System;

namespace ITJakub.DataEntities.Database.Entities
{
    public class ManuscriptDescription : IEquatable<ManuscriptDescription>
    {
        public virtual long Id { get; set; }
        public virtual string Country { get; set; }
        public virtual string Settlement { get; set; }
        public virtual string Idno { get; set; }
        public virtual string Title { get; set; }
        public virtual string OriginDate { get; set; }
        public virtual string Repository { get; set; }
        public virtual BookVersion BookVersion { get; set; }


        public virtual bool Equals(ManuscriptDescription other)
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
            return Equals((ManuscriptDescription) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}