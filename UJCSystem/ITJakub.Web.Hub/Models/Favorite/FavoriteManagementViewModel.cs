using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Favorite
{
    public class FavoriteManagementViewModel
    {
        public IList<FavoriteLabelViewModel> FavoriteLabels { get; set; }

        public IList<FavoriteSortViewModel> SortList { get; set; }

        public IList<FavoriteFilterViewModel> FilterList { get; set; }
    }
}