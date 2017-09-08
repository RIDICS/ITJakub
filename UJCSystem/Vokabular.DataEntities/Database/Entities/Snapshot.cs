using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class Snapshot : IEquatable<Snapshot>
    {
        public virtual long Id { get; set; }
        public virtual int VersionNumber { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual DateTime PublishTime { get; set; }
        public virtual string Comment { get; set; }
        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
        
        public virtual IList<ResourceVersion> ResourceVersions { get; set; }
        public virtual IList<BookType> BookTypes { get; set; }

        public virtual bool Equals(Snapshot other)
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
            return Equals((Snapshot) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}