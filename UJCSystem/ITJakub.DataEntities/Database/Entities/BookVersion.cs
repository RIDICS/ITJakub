using System;
using System.Collections.Generic;
using ITJakub.DataEntities.Database.Entities.Enums;

namespace ITJakub.DataEntities.Database.Entities
{
    public class BookVersion : IEquatable<BookVersion>
    {
        public virtual long Id { get; set; }
        public virtual Book Book { get; set; }
        public virtual Transformation Transformation { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual string VersionId { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string Description { get; set; }
        public virtual string PublishPlace { get; set; }
        public virtual string PublishDate { get; set; }
        public virtual string Copyright { get; set; }
        public virtual AvailabilityStatusEnum AvailabilityStatus { get; set; }
        public virtual string BiblText { get; set; }
        public virtual IList<BookBibl> BookBibls { get; set; }
        public virtual IList<Author> Authors { get; set; }
        public virtual IList<Responsible> Responsibles { get; set; }

        public virtual bool Equals(BookVersion other)
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
            return Equals((BookVersion) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}