using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.DataEntities.Database.Entities
{
    public class NewsSyndicationItem : IEquatable<NewsSyndicationItem>
    {
        public virtual long Id { get; set; }

        public virtual string Title { get; set; }

        public virtual string Text { get; set; }

        public virtual string Url { get; set; }

        public virtual User User { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual bool Equals(NewsSyndicationItem other)
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
            return Equals((NewsSyndicationItem) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
