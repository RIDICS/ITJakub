using System.Collections.Generic;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchSourceDetailViewModel
    {
        public string SearchTerm { get; set; }
        public bool ShowResults { get; set; }
        public List<SearchResultWithHtmlContext> SearchResults { get; set; }
    }
}