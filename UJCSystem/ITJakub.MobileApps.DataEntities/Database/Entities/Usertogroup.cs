using System;
using System.Text;
using System.Collections.Generic;


namespace ITJakub.MobileApps.DataEntities.Database.Entities {
    
    public class UserToGroup:IEquatable<UserToGroup>
    {
        public virtual long UserId { get; set; }
        public virtual long GroupId { get; set; }
        public virtual User User { get; set; }
        public virtual Group Group { get; set; }


        public virtual bool Equals(UserToGroup other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return UserId == other.UserId && GroupId == other.GroupId;
        }

        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserToGroup) obj);
        }
        public override int GetHashCode() {
            unchecked
            {
                return (UserId.GetHashCode()*397) ^ GroupId.GetHashCode();
            }
        }
        #endregion
    }
}
