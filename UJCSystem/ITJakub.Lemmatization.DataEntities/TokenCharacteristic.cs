using System;
using System.Collections.Generic;

namespace ITJakub.Lemmatization.DataEntities
{
    public class TokenCharacteristic : IEquatable<TokenCharacteristic>
    {
        public virtual long Id { get; set; }               

        public virtual IList<CanonicalForm> CanonicalForms { get; set; }

        public virtual bool Equals(TokenCharacteristic other)
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
            return Equals((TokenCharacteristic) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class HyperCanonicalForm
    {
        public virtual long Id { get; protected set; }

        public virtual IList<CanonicalForm> CanonicalForms { get; set; }


    }
}