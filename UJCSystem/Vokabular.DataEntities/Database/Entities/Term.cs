using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class Term : IEquatable<Term>
    {
        public virtual int Id { get; set; }

        public virtual string ExternalId { get; set; }

        public virtual string Text { get; set; }

        public virtual long Position { get; set; }

        //public virtual IList<BookPage> ReferencedFrom { get; set; }

        public virtual TermCategory TermCategory { get; set; }


        public virtual bool Equals(Term other)
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
            return Equals((Term) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}