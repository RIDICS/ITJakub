using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class ResponsiblePerson : IEquatable<ResponsiblePerson>
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual IList<ProjectResponsiblePerson> Projects { get; set; }

        public virtual bool Equals(ResponsiblePerson other)
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
            return Equals((ResponsiblePerson) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}