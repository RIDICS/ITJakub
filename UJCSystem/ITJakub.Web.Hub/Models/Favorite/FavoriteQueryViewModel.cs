using System;

namespace ITJakub.Web.Hub.Models.Favorite
{
    public class FavoriteQueryViewModel
    {
        public long Id { get; set; }
        
        public string Title { get; set; }

        public DateTime? CreateTime { get; set; }

        public string Query { get; set; }

        public FavoriteLabelViewModel FavoriteLabel { get; set; }
    }
}