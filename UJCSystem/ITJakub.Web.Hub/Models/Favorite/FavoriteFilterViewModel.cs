using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;

namespace ITJakub.Web.Hub.Models.Favorite
{
    public class FavoriteFilterViewModel
    {
        public FavoriteFilterViewModel(FavoriteTypeContract favoriteType, string name)
        {
            FavoriteType = favoriteType;
            Name = name;
        }

        public FavoriteTypeContract FavoriteType { get; set; }

        public string Name { get; set; }
    }
}