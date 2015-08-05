using System;
using System.Data;

namespace ITJakub.DataEntities.Database.Entities
{
    public class Note : IEquatable<Note>
    {
        public virtual long Id { get; set; }

        public virtual string Text { get; set; }

        public virtual User User { get; set; }

        public virtual DateTime CreateDate { get; set; }


        public virtual bool Equals(Note other)
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
            return Equals((Note) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class HeadwordNote:Note
    {
        public virtual BookHeadword BookHeadword { get; set; }
    }
}