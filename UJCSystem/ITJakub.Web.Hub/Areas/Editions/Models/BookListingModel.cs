using System.Collections.Generic;
using ITJakub.Shared.Contracts;

namespace ITJakub.Web.Hub.Areas.Editions.Models
{
    public class BookListingModel
    {
        public string BookXmlId { get; set; }
        public string BookTitle { get; set; }
        public IList<BookPageContract> BookPages { get; set; }
    }
}