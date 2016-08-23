using System;
using System.Collections.Generic;

namespace ITJakub.DataEntities.Database.Entities
{
    public class FavoriteLabel : IEquatable<FavoriteLabel>
    {
        public virtual long Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Color { get; set; }

        public virtual DateTime? LastUseTime { get; set; }

        public virtual FavoriteLabel ParentLabel { get; set; }

        public virtual IList<FavoriteLabel> SubLabels { get; set; }


        public virtual bool Equals(FavoriteLabel other)
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
            return Equals((FavoriteLabel) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}