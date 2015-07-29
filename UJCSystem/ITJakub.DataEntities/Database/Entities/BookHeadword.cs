using System;
using ITJakub.DataEntities.Database.Entities.Enums;

namespace ITJakub.DataEntities.Database.Entities
{
    public class BookHeadword : IEquatable<BookHeadword>
    {
        public virtual long Id { get; set; }
        public virtual BookVersion BookVersion { get; set; }
        public virtual string XmlEntryId { get; set; }
        public virtual string DefaultHeadword { get; set; }
        public virtual string Headword { get; set; }
        public virtual VisibilityEnum Visibility { get; set; }
        public virtual string Image { get; set; }
        public virtual string SortOrder { get; set; }

        public virtual bool Equals(BookHeadword other)
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
            return Equals((BookHeadword) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}