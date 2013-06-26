using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using System.Linq;

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


        public List<SelectionBase> GetCategoryTextSequence(string bookId, List<SelectionBase> parrents, SelectionBase currentCat)
        {
            var asCat = currentCat as Category;
            if (asCat != null)
            {
                if (asCat.Subitems.FirstOrDefault(x => x.Id == bookId) != null)
                    return parrents;

                
                foreach (var sublevel in asCat.Subitems)
                {
                    var listcopy = new List<SelectionBase>(parrents);
                    listcopy.Add(currentCat);
                    return GetCategoryTextSequence(bookId, listcopy, sublevel);
                }

            }
            return null;
        }
    }

    public class SearchKeyWordsViewModel
    {
        public List<SearchResultWithHtmlContext> Results { get; set; }
    }
}