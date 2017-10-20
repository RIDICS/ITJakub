using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.DataContracts
{
    public class ResultHeadwordListContract
    {
        public Dictionary<long, BookContract> BookList { get; set; }

        public List<HeadwordWithDictionariesContract> HeadwordList { get; set; }
    }

    public class HeadwordWithDictionariesContract
    {
        public string Headword { get; set; }

        public List<HeadwordBookInfoContract> Dictionaries { get; set; }
    }

    public class HeadwordBookInfoContract
    {
        public long HeadwordId { get; set; }

        public long HeadwordVersionId { get; set; }

        public long BookId { get; set; }
        
        public long? PageId { get; set; }
    }
}
