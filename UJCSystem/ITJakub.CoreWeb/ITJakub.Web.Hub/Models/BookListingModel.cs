using System.Collections.Generic;
using ITJakub.Shared.Contracts;
using Newtonsoft.Json;

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
        public bool CanPrintEdition { get; set; }
        public JsonSerializerSettings JsonSerializerSettingsForBiblModule { get; set; }
    }
}