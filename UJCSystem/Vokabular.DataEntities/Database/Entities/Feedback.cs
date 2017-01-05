using System;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.DataEntities.Database.Entities
{
    public class Feedback : IEquatable<Feedback>
    {
        public virtual FeedbackType FeedbackType { get { return FeedbackType.Generic; } }

        public virtual long Id { get; set; }

        public virtual string Text { get; set; }

        public virtual string AuthorName { get; set; }

        public virtual string AuthorEmail { get; set; }

        public virtual User AuthorUser { get; set; }

        public virtual DateTime CreateTime { get; set; }

        public virtual FeedbackCategoryEnum FeedbackCategory { get; set; }


        public virtual bool Equals(Feedback other)
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
            return Equals((Feedback) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class HeadwordFeedback : Feedback
    {
        public override FeedbackType FeedbackType { get { return FeedbackType.Headword; } }

        public virtual Resource HeadwordResource { get; set; }
    }
}