using System;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities.SqlServer.Entities
{
    public class SynchronizedObjectData : ISynchronizedObjectEntity, IEquatable<SynchronizedObjectData>
    {
        public virtual long Id { get; protected set; }

        public virtual long GroupId
        {
            get { return Group.Id; }
        }

        public virtual Group Group { get; set; }

        public virtual string ExternalId
        {
            get { return Convert.ToString(Id); }
        }

        public virtual string Data { get; set; }

        public virtual bool Equals(SynchronizedObjectData other)
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
            return Equals((SynchronizedObjectData) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}