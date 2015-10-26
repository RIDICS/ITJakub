using System;
using System.Collections.Generic;

namespace ITJakub.Lemmatization.DataEntities
{
    public class CanonicalForm : IEquatable<CanonicalForm>
    {
        public virtual long Id { get; set; }

        public virtual string Text { get; set; }

        public virtual string Description { get; set; }

        public virtual CanonicalFormType Type { get; set; }    
        
        public virtual HyperCanonicalForm HyperCanonicalForm { get; set; }

        public virtual IList<TokenCharacteristic> CanonicalFormFor { get; set; }

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

    public enum CanonicalFormType : short
    {
        Lemma = 0,
        Stemma = 1,
        LemmaOld = 2,
        StemmaOld = 3,
    }
}