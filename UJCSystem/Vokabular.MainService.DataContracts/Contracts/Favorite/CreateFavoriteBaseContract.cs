using Vokabular.MainService.DataContracts.Contracts.Favorite.Type;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.DataContracts.Contracts.Favorite
{
    public abstract class CreateFavoriteBaseContract
    {
        public string Title { get; set; }

        //public string Description { get; set; }

        public long FavoriteLabelId { get; set; }
    }

    public class CreateFavoriteProjectContract : CreateFavoriteBaseContract
    {
        public long ProjectId { get; set; }
    }

    public class CreateFavoriteCategoryContract : CreateFavoriteBaseContract
    {
        public int CategoryId { get; set; }
    }

    public class CreateFavoriteQueryContract : CreateFavoriteBaseContract
    {
        public BookTypeEnumContract BookType { get; set; }

        public QueryTypeEnumContract QueryType { get; set; }

        public string Query { get; set; }
    }

    public class CreateFavoritePageContract : CreateFavoriteBaseContract
    {
        public long PageId { get; set; }
    }
}
