using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class FavoriteLabel : IEquatable<FavoriteLabel>
    {
        public virtual long Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Color { get; set; }

        public virtual bool IsDefault { get; set; }

        public virtual DateTime? LastUseTime { get; set; }

        public virtual User User { get; set; }

        public virtual IList<FavoriteBase> FavoriteItems { get; set; }


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