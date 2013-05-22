using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }
        public string[] SearchPart { get; set; }
        public SearchResult[] SearchedBooks { get; set; }
    }
}