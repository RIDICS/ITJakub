using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ProjectGroup : IEquatable<ProjectGroup>
    {
        public virtual int Id { get; set; }

        public virtual DateTime CreateTime { get; set; }

        public virtual IList<Project> Projects { get; set; }


        public virtual bool Equals(ProjectGroup other)
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
            return Equals((ProjectGroup) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}