using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ProjectOriginalAuthor : IEquatable<ProjectOriginalAuthor>
    {
        private ProjectOriginalAuthorId m_projectOriginalAuthorId = new ProjectOriginalAuthorId();
        private Project m_project;
        private OriginalAuthor m_originalAuthor;

        public virtual ProjectOriginalAuthorId ProjectOriginalAuthorId
        {
            get => m_projectOriginalAuthorId;
            set => m_projectOriginalAuthorId = value;
        }

        public virtual Project Project
        {
            get { return m_project; }
            set
            {
                m_project = value;
                m_projectOriginalAuthorId.ProjectId = value.Id;
            }
        }

        public virtual OriginalAuthor OriginalAuthor
        {
            get { return m_originalAuthor; }
            set
            {
                m_originalAuthor = value;
                m_projectOriginalAuthorId.OriginalAuthorId = value.Id;
            }
        }

        public virtual int Sequence { get; set; }

        public virtual bool Equals(ProjectOriginalAuthor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ProjectOriginalAuthorId, other.ProjectOriginalAuthorId);
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
            return (ProjectOriginalAuthorId != null ? ProjectOriginalAuthorId.GetHashCode() : 0);
        }
    }

    public class ProjectOriginalAuthorId : IEquatable<ProjectOriginalAuthorId>
    {
        public virtual long ProjectId { get; set; }

        public virtual int OriginalAuthorId { get; set; }

        public virtual bool Equals(ProjectOriginalAuthorId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ProjectId, other.ProjectId) && Equals(OriginalAuthorId, other.OriginalAuthorId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ProjectOriginalAuthorId)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ProjectId.GetHashCode() * 397) ^ OriginalAuthorId.GetHashCode();
            }
        }
    }
}