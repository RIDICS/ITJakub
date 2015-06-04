using System;
using System.Collections.Generic;

namespace ITJakub.DataEntities.Database.Entities
{
    public class BookContentItem : IEquatable<BookContentItem>
    {
        public virtual long Id { get; set; }
        public virtual string Text { get; set; }
        public virtual BookVersion BookVersion { get; set; }
        public virtual BookPage Page { get; set; }
        public virtual BookContentItem ParentBookContentItem { get; set; }
        public virtual IList<BookContentItem> ChildContentItems { get; set; }
        public virtual int ItemOrder { get; set; }

        public virtual bool Equals(BookContentItem other)
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
            return Equals((BookContentItem)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}