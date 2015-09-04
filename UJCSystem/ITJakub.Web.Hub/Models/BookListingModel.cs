using System.Collections.Generic;
using ITJakub.Shared.Contracts;

namespace ITJakub.Web.Hub.Models
{
    public class BookListingModel
    {
        public long BookId { get; set; }
        public string BookXmlId { get; set; }
        public string VersionXmlId { get; set; }
        public string BookTitle { get; set; }
        public IList<BookPageContract> BookPages { get; set; }
        public string SearchText { get; set; }
        public string InitPageXmlId { get; set; }
    }
}