using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Favorite
{
    public class CreateFavoriteBookRequest
    {
        public long BookId { get; set; }
        public string Title { get; set; }
        public IList<long> LabelIds { get; set; }
    }
}
