using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Favorite
{
    public class NewFavoriteViewModel
    {
        public string ItemName { get; set; }

        public IList<FavoriteLabelViewModel> Labels { get; set; }
    }
}