using System;
using Vokabular.MainService.DataContracts.Contracts.Favorite.Type;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.DataContracts.Contracts.Favorite
{
    public class FavoriteBaseInfoContract
    {
        public long Id { get; set; }

        public string Title { get; set; }

        //public string Description { get; set; }

        public FavoriteTypeEnumContract FavoriteType { get; set; }

        public DateTime CreateTime { get; set; }

        public long FavoriteLabelId { get; set; }
    }

    public class FavoriteBaseWithLabelContract : FavoriteBaseInfoContract
    {
        public FavoriteLabelContract FavoriteLabel { get; set; }
    }

    public class FavoriteFullInfoContract : FavoriteBaseInfoContract
    {
        public long? ProjectId { get; set; }

        public long? PageId { get; set; }

        public int? CategoryId { get; set; }

        public BookTypeEnumContract? BookType { get; set; }

        public QueryTypeEnumContract? QueryType { get; set; }

        public string Query { get; set; }
    }

    public class FavoriteQueryContract : FavoriteBaseInfoContract
    {
        public string Query { get; set; }

        public BookTypeEnumContract BookType { get; set; }

        public QueryTypeEnumContract QueryType { get; set; }

        public FavoriteLabelContract FavoriteLabel { get; set; }
    }

    public class FavoritePageContract : FavoriteBaseInfoContract
    {
        public long PageId { get; set; }

        public FavoriteLabelContract FavoriteLabel { get; set; }
    }
}
