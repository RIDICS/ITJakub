using System;
using ITJakub.DataEntities.Database.Entities.Enums;

namespace ITJakub.DataEntities.Database.Entities
{
    public class Feedback : IEquatable<Feedback>
    {
        public virtual long Id { get; set; }

        public virtual string Text { get; set; }

        public virtual string Name { get; set; }

        public virtual string Email { get; set; }

        public virtual User User { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual FeedbackCategoryEnum Category { get; set; }


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

    public class HeadwordFeedback:Feedback
    {
        public virtual BookHeadword BookHeadword { get; set; }
    }
}