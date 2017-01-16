using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ProjectOriginalAuthor : IEquatable<ProjectOriginalAuthor>
    {
        public virtual Project Project { get; set; }
        public virtual OriginalAuthor OriginalAuthor { get; set; }
        public virtual int Sequence { get; set; }

        public virtual bool Equals(ProjectOriginalAuthor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Project, other.Project) && Equals(OriginalAuthor, other.OriginalAuthor);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ProjectOriginalAuthor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Project != null ? Project.GetHashCode() : 0)*397) ^ (OriginalAuthor != null ? OriginalAuthor.GetHashCode() : 0);
            }
        }
    }
}