using System;


namespace ITJakub.MobileApps.DataEntities.Database.Entities {
    
    public abstract class SynchronizedObjectBase:IEquatable<SynchronizedObjectBase>
    {
        public virtual long Id { get; set; }

        public virtual User Author { get; set; }

        public virtual Group Group { get; set; }

        public virtual Application Application { get; set; }

        public virtual DateTime CreateTime { get; set; }

        public virtual string ObjectType { get; set; }
   
        public virtual string Data { get; set; }    //not mapped because its stored in azure tables


        public virtual bool Equals(SynchronizedObjectBase other)
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
            return Equals((SynchronizedObjectBase) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SynchronizedObject : SynchronizedObjectBase
    {
        public virtual string ObjectExternalId { get; set; }
    }

    public class SingleSynchronizedObject : SynchronizedObjectBase
    {
        public virtual string ObjectValue { get; set; }
    }
}
