using System.Collections.Generic;
using ITJakub.Contracts.Categories;
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

        public IEnumerable<string> GetAllCategoryCombinations()
        {
            List<string> result = new List<string>();
            foreach (var foundInBook in FoundInBooks)
            {
                foreach (var categoryCombination in foundInBook.TextCategoriesClassification)
                {
                    if (!result.Contains(categoryCombination))
                        result.Add(categoryCombination);    
                }
            }
            return result;
        }

        public IEnumerable<Book> GetBooksByCategoryCombination(string categoryCombination)
        {
            return FoundInBooks.Where(x => x.TextCategoriesClassification.Contains(categoryCombination));
        }
    }
}