using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.DataEntities.Database.Entities
{
    public class Resource : IEquatable<Resource>
    {
        public virtual long Id { get; set; }

        public virtual string Name { get; set; }

        public virtual ResourceTypeEnum ResourceType { get; set; }

        public virtual ContentTypeEnum ContentType { get; set; }

        public virtual Project Project { get; set; }

        public virtual IList<ResourceVersion> ResourceVersions { get; set; }

        public virtual ResourceVersion LatestVersion { get; set; }
        
        public virtual bool Equals(Resource other)
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
            return Equals((Resource) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}