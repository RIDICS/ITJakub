using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.DataEntities.Database.Entities
{
    public class NamedResourceGroup : IEquatable<NamedResourceGroup>
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual TextTypeEnum TextType { get; set; }
        public virtual Project Project { get; set; }
        public virtual IList<Resource> Resources { get; set; }


        public virtual bool Equals(NamedResourceGroup other)
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
            return Equals((NamedResourceGroup) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}