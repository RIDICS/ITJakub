using ITJakub.Shared.Contracts.Favorites;

namespace ITJakub.Web.Hub.Models.Favorite
{
    public class FavoriteSortViewModel
    {
        public FavoriteSortViewModel(FavoriteSortContract sortType, string name)
        {
            SortType = sortType;
            Name = name;
        }

        public FavoriteSortContract SortType { get; set; }

        public string Name { get; set; }
    }
}