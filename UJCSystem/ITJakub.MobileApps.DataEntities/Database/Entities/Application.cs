using System;
using System.Text;
using System.Collections.Generic;


namespace ITJakub.MobileApps.DataEntities.Database.Entities {
    
    public class Application:IEquatable<Application>
    {
        public Application() { }
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }

        public virtual IList<Task> Tasks { get; set; }

        public virtual bool Equals(Application other)
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
            return Equals((Application) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
