using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class Keyword : IEquatable<Keyword>
    {
        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public virtual IList<Project> Projects { get; set; }

        public virtual bool Equals(Keyword other)
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
            return Equals((Keyword)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}