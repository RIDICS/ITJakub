using System.Collections.Generic;
using System.Linq;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchKeyWordsViewModel
    {
        public List<SearchResultWithHtmlContext> Results { get; set; }

        public IEnumerable<string> GetAllPossibleCategories()
        {
            var categories = new List<string>();
            foreach (var result in Results)
            {
                foreach (var resultCat in result.Categories.Where(resultCat => !categories.Contains(resultCat)))
                {
                    categories.Add(resultCat);
                }
                
            }
            return categories;
        }
    }
}