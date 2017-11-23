using Vokabular.Shared.DataContracts.Types.Favorite;

namespace ITJakub.Web.Hub.Models.Favorite
{
    public class FavoriteFilterViewModel
    {
        public FavoriteFilterViewModel(FavoriteTypeEnumContract favoriteType, string name)
        {
            FavoriteType = favoriteType;
            Name = name;
        }

        public FavoriteTypeEnumContract FavoriteType { get; set; }

        public string Name { get; set; }
    }
}