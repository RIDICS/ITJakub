using System;

namespace ITJakub.Lemmatization.DataEntities
{
    public class CanonicalForm : IEquatable<CanonicalForm>
    {
        public virtual long Id { get; set; }

        public virtual string Text { get; set; }

        public virtual CanonicalFormType Type { get; set; }        

        public virtual bool Equals(CanonicalForm other)
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
            return Equals((CanonicalForm) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public enum CanonicalFormType
    {
        Lemma,
        Stemma,
        LemmaOld,
        StemmaOld,
        HyperLemma,
        HyperStemma,
    }
}