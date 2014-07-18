using System;

namespace ITJakub.MobileApps.DataEntities.Database.Entities {
    
    public class UserToGroup:IEquatable<UserToGroup>
    {
        public virtual long Id { get; set; }
        public virtual User User { get; set; }
        public virtual Group Group { get; set; }

        public virtual bool Equals(UserToGroup other)
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
            return Equals((UserToGroup) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
