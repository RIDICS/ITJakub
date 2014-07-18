using System;
using System.Collections.Generic;

namespace ITJakub.MobileApps.DataEntities.Database.Entities
{
    public class Application : IEquatable<Application>
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }

        public virtual List<Task> Tasks { get; set; }

        public virtual bool Equals(Application other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Application) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}