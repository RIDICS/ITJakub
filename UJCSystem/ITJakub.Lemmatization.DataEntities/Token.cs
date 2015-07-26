using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.Lemmatization.DataEntities
{
    public class Token : IEquatable<Token>
    {
        public virtual long Id { get; protected set; }

        public virtual string Text { get; set; }

        public virtual IList<TokenCharacteristic> TokenCharacteristics { get; set; }

        public virtual bool Equals(Token other)
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
            return Equals((Token) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
