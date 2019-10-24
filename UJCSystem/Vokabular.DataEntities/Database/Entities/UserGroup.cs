using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class UserGroup : IEquatable<UserGroup>
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
        
        public virtual DateTime CreateTime { get; set; }

        public virtual DateTime LastChange { get; set; }

        public virtual IList<User> Users { get; set; }

        public virtual IList<Permission> Permissions { get; set; }

        public virtual bool Equals(UserGroup other)
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
            return Equals((UserGroup) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SingleUserGroup : UserGroup
    {
        public virtual User User { get; set; }
    }

    public class RoleUserGroup : UserGroup
    {
        public virtual int ExternalId { get; set; }
    }
}