using System;
using ITJakub.DataEntities.Database.Entities.Enums;

namespace ITJakub.DataEntities.Database.Entities
{
    public abstract class FavoriteBase : IEquatable<FavoriteBase>
    {
        public abstract FavoriteTypeEnum FavoriteTypeEnum { get; }

        public virtual string FavoriteType { get; protected set; }

        public virtual long Id { get; set; }

        public virtual User User { get; set; }

        public virtual string Title { get; set; }

        public virtual FavoriteLabel FavoriteLabel { get; set; }

        public virtual DateTime? CreateTime { get; set; }

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
            return Id.GetHashCode();
        }
    }

    public class PageBookmark : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum
        {
            get { return FavoriteTypeEnum.PageBookmark; }
        }

        public virtual string PageXmlId { get; set; }

        public virtual int PagePosition { get; set; }
        
        public virtual Book Book { get; set; }
    }

    public class FavoriteCategory : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum
        {
            get { return FavoriteTypeEnum.Category; }
        }

        public virtual Category Category { get; set; }
    }

    public class FavoriteBook : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum
        {
            get { return FavoriteTypeEnum.Book; }
        }

        public virtual Book Book { get; set; }
    }

    public class FavoriteBookVersion : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum
        {
            get { return FavoriteTypeEnum.BookVersion; }
        }

        public virtual BookVersion BookVersion { get; set; }
    }

    public class HeadwordBookmark : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum
        {
            get { return FavoriteTypeEnum.HeadwordBookmark; }
        }

        public virtual Book Book { get; set; }

        public virtual string XmlEntryId { get; set; }
    }

    public class FavoriteQuery : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum
        {
            get { return FavoriteTypeEnum.Query; }
        }

        public virtual BookType BookType { get; set; }

        public virtual QueryTypeEnum QueryType { get; set; }

        public virtual string Query { get; set; }
    }
}