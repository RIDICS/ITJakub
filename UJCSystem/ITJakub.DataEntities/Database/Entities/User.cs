using System;
using System.Collections.Generic;
using ITJakub.DataEntities.Database.Entities.Enums;

namespace ITJakub.DataEntities.Database.Entities
{
    public class User : IEquatable<User>
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual AuthenticationProviderEnum AuthenticationProvider { get; set; }
        public virtual string CommunicationToken { get; set; }
        public virtual DateTime? CommunicationTokenCreateTime { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string Salt { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string AvatarUrl { get; set; }
        public virtual IList<Bookmark> Bookmarks { get; set; }

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