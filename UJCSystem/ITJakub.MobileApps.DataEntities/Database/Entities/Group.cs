using System;
using System.Collections.Generic;

namespace ITJakub.MobileApps.DataEntities.Database.Entities
{
    public class Group : IEquatable<Group>
    {
        public virtual long Id { get; set; }
        public virtual Task Task { get; set; }
        public virtual Institution Institution { get; set; }

        public virtual User Author { get; set; }
        public virtual List<User> Users { get; set; }

        public virtual List<SynchronizedObject> SynchronizedObjects { get; set; }
        public virtual DateTime CreateTime { get; set; }

        public virtual bool Equals(Group other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Equals(Task, other.Task) && Equals(Institution, other.Institution);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Group) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ (Task != null ? Task.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Institution != null ? Institution.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}