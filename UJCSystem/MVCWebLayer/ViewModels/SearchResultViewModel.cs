using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel()
        {
        }

        public SearchViewModel Search { get; set; }
        public string[] FoundWords { get; set; }

        public SearchResult[] Results { get; set; }

        public List<SelectionBase> Categories { get; set; }
    }

    public class SearchKeyWordsViewModel
    {
        public SearchResult[] Results { get; set; }
    }
}