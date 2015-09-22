using System;
using System.Collections.Generic;


namespace ITJakub.MobileApps.DataEntities.Database.Entities
{
    public class Task : IEquatable<Task>
    {
        public Task()
        {
        }

        public virtual long Id { get; set; }

        public virtual User Author { get; set; }

        public virtual Application Application { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime CreateTime { get; set; }

        public virtual string Description { get; set; }

        public virtual IList<Group> Groups { get; set; }

        public virtual string Data { get; set; } //not mapped because its stored in azure tables

        public virtual bool Equals(Task other)
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
            return Equals((Task) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}