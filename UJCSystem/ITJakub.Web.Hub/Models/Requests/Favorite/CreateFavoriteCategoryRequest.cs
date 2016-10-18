using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Favorite
{
    public class CreateFavoriteCategoryRequest
    {
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public IList<long> LabelIds { get; set; }
    }
}