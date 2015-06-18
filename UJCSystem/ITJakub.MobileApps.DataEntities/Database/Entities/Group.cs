using System;
using System.Collections.Generic;

namespace ITJakub.MobileApps.DataEntities.Database.Entities
{
    public class Group : IEquatable<Group>
    {
        public virtual long Id { get; set; }

        public virtual string Name { get; set; }

        public virtual User Author { get; set; }

        public virtual Task Task { get; set; }

        public virtual DateTime CreateTime { get; set; }

        public virtual string EnterCode { get; set; }

        public virtual string Evaluation { get; set; }

        public virtual GroupState State { get; set; }

        public virtual IList<SynchronizedObjectBase> SynchronizedObjects { get; set; }

        public virtual IList<User> Members { get; set; }

        public virtual bool Equals(Group other)
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
            return Equals((Group) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}