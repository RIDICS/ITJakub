using System;
using System.Text;
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
        public virtual DateTime CreateTime { get; set; }


        public virtual List<Group> Groups { get; set; }     //Groups made by this user
        public virtual List<SynchronizedObject> SynchronizedObjects { get; set; } //Sync objects made by this user
        public virtual List<Task> Tasks { get; set; }   //Tasks made by this user

        public virtual List<UserToGroup> UserToGroup { get; set; }

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
