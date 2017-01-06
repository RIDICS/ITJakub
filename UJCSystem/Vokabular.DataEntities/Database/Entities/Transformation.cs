using System;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.DataEntities.Database.Entities
{
    public class Transformation : IEquatable<Transformation>
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual BookType BookType { get; set; }
        public virtual OutputFormatEnum OutputFormat { get; set; }
        public virtual ResourceLevelEnum ResourceLevel { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsDefaultForBookType { get; set; }
        

        public virtual bool Equals(Transformation other)
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
            return Equals((Transformation) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
