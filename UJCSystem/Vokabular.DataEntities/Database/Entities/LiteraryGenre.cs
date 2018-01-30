using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities.SelectResults;

namespace Vokabular.DataEntities.Database.Entities
{
    public class LiteraryGenre : IEquatable<LiteraryGenre>, ICatalogValue
    {
        public virtual int Id { get; set; }
        
        public virtual string Name { get; set; }

        public virtual IList<Project> Projects { get; set; }

        public virtual bool Equals(LiteraryGenre other)
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
            return Equals((LiteraryGenre) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}