using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Favorite
{
    public class GetFavoriteLabeledBookRequest
    {
        public IList<long> BookIds { get; set; }
    }

    public class GetFavoriteLabeledCategoryRequest
    {
        public IList<int> CategoryIds { get; set; }
    }
}
