using System;
using System.Collections.Generic;
using ITJakub.DataEntities.Database.Entities.Enums;

namespace ITJakub.DataEntities.Database.Entities
{
    public class ResponsibleType : IEquatable<ResponsibleType>
    {
        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public virtual ResponsibleTypeEnum Type { get; set; }
        public virtual IList<Responsible> Responsibles { get; set; }

        public virtual bool Equals(ResponsibleType other)
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
            return Equals((ResponsibleType) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}