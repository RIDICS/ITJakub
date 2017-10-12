using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities {
    
    public class Category : IEquatable<Category>
    {
        public virtual int Id { get; set; }

        public virtual Category ParentCategory { get; set; }

        public virtual string ExternalId { get; set; }

        public virtual string Description { get; set; }

        public virtual string Path { get; set; }

        public virtual IList<Category> Categories { get; set; }

        public virtual IList<Project> Projects { get; set; }

        //public virtual IList<FavoriteCategory> FavoriteItems { get; set; }

        public virtual bool Equals(Category other)
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
            return Equals((Category) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
