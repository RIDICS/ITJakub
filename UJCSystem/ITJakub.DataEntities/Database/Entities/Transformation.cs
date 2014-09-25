using System;
using System.Collections.Generic;

namespace ITJakub.DataEntities.Database.Entities {
    
    public class Transformation : IEquatable<Transformation> {

        public virtual int Id { get; set; }
        public virtual Booktype BookType { get; set; }
        public virtual short ResultType { get; set; }
        public virtual bool IsDefault { get; set; }
        public virtual IList<BookVersion> BookVersions { get; set; }

        public bool Equals(Transformation other)
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
            return Equals((Transformation) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
