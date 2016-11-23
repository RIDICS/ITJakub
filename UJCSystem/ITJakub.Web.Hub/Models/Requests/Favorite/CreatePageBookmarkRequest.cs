using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Favorite
{
    public class CreatePageBookmarkRequest
    {
        public string BookXmlId { get; set; }
        public string PageXmlId { get; set; }
        public string Title { get; set; }
        public IList<long> LabelIds { get; set; }
    }
}