using Vokabular.Shared.DataContracts.Types.Favorite;

namespace ITJakub.Web.Hub.Models.Favorite
{
    public class FavoriteSortViewModel
    {
        public FavoriteSortViewModel(FavoriteSortEnumContract sortType, string name)
        {
            SortType = sortType;
            Name = name;
        }

        public FavoriteSortEnumContract SortType { get; set; }

        public string Name { get; set; }
    }
}