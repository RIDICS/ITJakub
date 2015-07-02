using System.Collections.Generic;

namespace ITJakub.Web.Hub.Areas.Dictionaries.Models
{
    public class GetHeadwordsDescription
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public IList<long> SelectedBookIds { get; set; }
    }
}