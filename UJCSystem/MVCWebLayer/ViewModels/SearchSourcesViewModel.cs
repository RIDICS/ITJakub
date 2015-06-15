using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchSourcesViewModel
    {
        public string SearchTerm { get; set; }
        public IEnumerable<SearchResult> FoundSources { get; set; }
    }
}