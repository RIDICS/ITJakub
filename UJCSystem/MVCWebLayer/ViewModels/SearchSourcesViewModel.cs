using System.Collections.Generic;
using ITJakub.Contracts.Categories;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchSourcesViewModel
    {
        public string SearchTerm { get; set; }
        public IEnumerable<Book> FoundSources { get; set; }
    }
}