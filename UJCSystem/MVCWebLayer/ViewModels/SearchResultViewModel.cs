using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel()
        {
        }

        public string SearchTerm { get; set; }
        public string[] FoundWords { get; set; }

        public SearchResult[] Results { get; set; }
    }

    public class SearchKeyWordsViewModel
    {
        public SearchResult[] Results { get; set; }
    }
}