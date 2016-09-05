using System;

namespace ITJakub.Web.Hub.Models.Favorite
{
    public class FavoriteLabelViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public DateTime? LastUseTime { get; set; }
    }
}