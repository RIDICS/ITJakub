using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ProjectResponsiblePerson : IEquatable<ProjectResponsiblePerson>
    {
        private ProjectResponsiblePersonId m_projectResponsiblePersonId = new ProjectResponsiblePersonId();
        private Project m_project;
        private ResponsiblePerson m_responsiblePerson;
        private ResponsibleType m_responsibleType;

        public virtual ProjectResponsiblePersonId ProjectResponsiblePersonId
        {
            get => m_projectResponsiblePersonId;
            set => m_projectResponsiblePersonId = value;
        }

        public virtual Project Project
        {
            get { return m_project; }
            set
            {
                m_project = value;
                m_projectResponsiblePersonId.ProjectId = value.Id;
            }
        }

        public virtual ResponsiblePerson ResponsiblePerson
        {
            get { return m_responsiblePerson; }
            set
            {
                m_responsiblePerson = value;
                m_projectResponsiblePersonId.ResponsiblePersonId = value.Id;
            }
        }

        public virtual ResponsibleType ResponsibleType
        {
            get { return m_responsibleType; }
            set
            {
                m_responsibleType = value;
                m_projectResponsiblePersonId.ResponsibleTypeId = value.Id;
            }
        }

        public virtual bool Equals(ProjectResponsiblePerson other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(m_projectResponsiblePersonId, other.m_projectResponsiblePersonId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectResponsiblePerson) obj);
        }

        public override int GetHashCode()
        {
            return (m_projectResponsiblePersonId != null ? m_projectResponsiblePersonId.GetHashCode() : 0);
        }
    }

    public class ProjectResponsiblePersonId : IEquatable<ProjectResponsiblePersonId>
    {
        public virtual long ProjectId { get; set; }

        public virtual int ResponsiblePersonId { get; set; }

        public virtual int ResponsibleTypeId { get; set; }

        public virtual bool Equals(ProjectResponsiblePersonId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ProjectId, other.ProjectId) && Equals(ResponsiblePersonId, other.ResponsiblePersonId) && Equals(ResponsibleTypeId, other.ResponsibleTypeId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ProjectResponsiblePersonId)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ProjectId.GetHashCode();
                hashCode = (hashCode * 397) ^ ResponsiblePersonId.GetHashCode();
                hashCode = (hashCode * 397) ^ ResponsibleTypeId.GetHashCode();
                return hashCode;
            }
        }
    }
}