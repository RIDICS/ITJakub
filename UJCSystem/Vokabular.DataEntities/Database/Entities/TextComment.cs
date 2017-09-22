using System;
using System.Collections.Generic;

namespace Vokabular.DataEntities.Database.Entities
{
    public class TextComment : IEquatable<TextComment>
    {
        public virtual long Id { get; set; }

        public virtual string TextReferenceId { get; set; }

        public virtual string Text { get; set; }

        public virtual DateTime CreateTime { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual TextComment ParentComment { get; set; }

        public virtual Resource ResourceText { get; set; }

        public virtual IList<TextComment> TextComments { get; set; }

        public virtual bool Equals(TextComment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TextComment) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}