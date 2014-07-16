using System;

namespace ITJakub.MobileApps.DataEntities.Database.Entities {
    
    public class UserToInstitution:IEquatable<UserToInstitution>
    {
        public virtual long Id { get; set; }
        public virtual User User { get; set; }
        public virtual Institution Institution { get; set; }

        public bool Equals(UserToInstitution other)
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
            return Equals((UserToInstitution) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
