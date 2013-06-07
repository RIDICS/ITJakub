using System.Web.Mvc;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchResultViewModel
    {
        public string SearchTerm { get; set; }
        public string[] FoundWords { get; set; }

        public SearchResult[] Results { get; set; }

        public SearchViewModel Search { get; set; }
    }

    public class SearchKeyWordsViewModel
    {
        public SearchResult[] Results { get; set; }
    }
}