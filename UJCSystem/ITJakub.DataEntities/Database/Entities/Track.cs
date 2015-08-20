using System;
using System.Collections.Generic;

namespace ITJakub.DataEntities.Database.Entities
{
    public class Track : IEquatable<Track>
    {
        public virtual long Id { get; protected set; }

        public virtual BookVersion BookVersion { get; set; }

        public virtual  string Name { get; set; }

        public virtual int Position { get; set; }

        public virtual IList<Recording> Recordings { get; set; }

        public virtual bool Equals(Track other)
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
            return Equals((Track) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

}