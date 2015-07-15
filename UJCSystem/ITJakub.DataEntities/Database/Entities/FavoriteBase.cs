using System;

namespace ITJakub.DataEntities.Database.Entities
{
    public abstract class FavoriteBase : IEquatable<FavoriteBase>
    {
        public virtual int Id { get; set; }

        public virtual User User { get; set; }

        public virtual bool Equals(FavoriteBase other)
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
            return Equals((FavoriteBase) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class PageBookmark : FavoriteBase
    {
        public virtual string PageXmlId { get; set; }

        public virtual int PagePosition { get; set; }

        public virtual Book Book { get; set; }
    }

    public class FavoriteCategory : FavoriteBase
    {
        public virtual Category Category { get; set; }
    }

    public class FavoriteBook : FavoriteBase
    {
        public virtual Book Book { get; set; }
    }

    public class HeadwordBookmark : FavoriteBase
    {
        public virtual Book Book { get; set; }

        public virtual string XmlEntryId { get; set; }
    }
}