using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class User : IEquatable<User>
    {
        public virtual int Id { get; set; }

        public virtual int ExternalId { get; set; }

        public virtual DateTime CreateTime { get; set; }

        public virtual string AvatarUrl { get; set; }

        //public virtual IList<FavoriteBase> FavoriteItems { get; set; }

        //public virtual IList<FavoriteLabel> FavoriteLabels { get; set; }

        public virtual IList<UserGroup> Groups { get; set; }
        

        public virtual bool Equals(User other)
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
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}