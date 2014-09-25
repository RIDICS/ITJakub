using System;
using System.Collections.Generic;

namespace ITJakub.DataEntities.Database.Entities {
    
    public class BookType : IEquatable<BookType> {
        public virtual int Id { get; set; }
        public virtual string Type { get; set; }
        public virtual IList<Book> Books { get; set; }
        public virtual IList<Transformation> Transformations { get; set; }

        public virtual bool Equals(BookType other)
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
            return Equals((BookType) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
