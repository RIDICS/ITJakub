using System;
using System.Collections.Generic;


namespace ITJakub.MobileApps.DataEntities.Database.Entities {
    
    public class User:IEquatable<User>
    {
        public User() { }
        public virtual long Id { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string AvatarUrl { get; set; }
        public virtual byte AuthenticationProvider { get; set; }
        public virtual string AuthenticationProviderToken { get; set; }
        public virtual string CommunicationToken { get; set; }
        public virtual DateTime CreateTime { get; set; }  
        public virtual DateTime CommunicationTokenCreateTime { get; set; } //TODO add to DB
        public virtual string PasswordHash { get; set; }
        public virtual string Salt { get; set; }
        public virtual IList<Group> CreatedGroups { get; set; }   
        protected virtual IList<SynchronizedObject> CreatedSynchronizedObjects { get; set; } 
        public virtual IList<Task> CreatedTasks { get; set; }   
        public virtual IList<Group> MemberOfGroups { get; set; }

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
            if (obj.GetType() != this.GetType()) return false;
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
