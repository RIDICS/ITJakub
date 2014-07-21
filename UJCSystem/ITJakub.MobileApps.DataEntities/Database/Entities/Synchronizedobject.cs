using System;
using System.Text;
using System.Collections.Generic;


namespace ITJakub.MobileApps.DataEntities.Database.Entities {
    
    public class SynchronizedObject:IEquatable<SynchronizedObject>
    {
        public virtual long Id { get; set; }
        public virtual User User { get; set; }
        public virtual Group Group { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string ObjectType { get; set; }
        public virtual string Guid { get; set; }

        public bool Equals(SynchronizedObject other)
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
            return Equals((SynchronizedObject) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
