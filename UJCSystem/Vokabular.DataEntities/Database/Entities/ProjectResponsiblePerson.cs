using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ProjectResponsiblePerson : IEquatable<ProjectResponsiblePerson>
    {
        public virtual Project Project { get; set; }
        public virtual ResponsiblePerson ResponsiblePerson { get; set; }
        public virtual ResponsibleType ResponsibleType { get; set; }

        public virtual bool Equals(ProjectResponsiblePerson other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Project, other.Project) && Equals(ResponsiblePerson, other.ResponsiblePerson) && Equals(ResponsibleType, other.ResponsibleType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ProjectResponsiblePerson) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Project != null ? Project.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ResponsiblePerson != null ? ResponsiblePerson.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ResponsibleType != null ? ResponsibleType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}