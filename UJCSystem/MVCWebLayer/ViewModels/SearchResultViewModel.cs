using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel()
        {
            Search = new SearchViewModel();
        }

        public SearchViewModel Search { get; set; }
        public List<string> FoundWords { get; set; }
        public List<Book> FoundInBooks { get; set; }

        //public SearchResult[] Results { get; set; }

        public List<SelectionBase> Categories { get; set; }

        public List<string> SelectedCategoryIds { get; set; }

        public List<string> SelectedBookIds { get; set; }

        public string CategoryIds { get; set; }

        public string BookIds { get; set; }
        
    }

    public class SearchKeyWordsViewModel
    {
        public List<SearchResultWithHtmlContext> Results { get; set; }
    }
}