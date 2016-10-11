using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Favorite
{
    public class FavoriteQueriesViewModel
    {
        public IList<FavoriteLabelViewModel> FavoriteLabelList { get; set; }

        public IList<FavoriteQueryViewModel> QueryList { get; set; }
    }
}