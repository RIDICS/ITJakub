using System;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.DataEntities.Database.Entities
{
    public abstract class FavoriteBase : IEquatable<FavoriteBase>
    {
        public abstract FavoriteTypeEnum FavoriteTypeEnum { get; }

        public virtual string FavoriteType { get; protected set; }

        public virtual long Id { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

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

    public class FavoritePage : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum => FavoriteTypeEnum.Page;

        public virtual Resource ResourcePage { get; set; }
    }

    public class FavoriteCategory : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum => FavoriteTypeEnum.Category;

        public virtual Category Category { get; set; }
    }

    public class FavoriteProject : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum => FavoriteTypeEnum.Project;

        public virtual Project Project { get; set; }
    }

    public class FavoriteSnapshot : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum => FavoriteTypeEnum.Snapshot;

        //TODO public virtual Snapshot Snapshot { get; set; }
    }

    public class FavoriteHeadword : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum => FavoriteTypeEnum.Headword;

        public virtual Resource DefaultHeadwordResource { get; set; }
    }

    public class FavoriteQuery : FavoriteBase
    {
        public override FavoriteTypeEnum FavoriteTypeEnum => FavoriteTypeEnum.Query;

        public virtual BookType BookType { get; set; }

        public virtual QueryTypeEnum QueryType { get; set; }

        public virtual string Query { get; set; }
    }
}