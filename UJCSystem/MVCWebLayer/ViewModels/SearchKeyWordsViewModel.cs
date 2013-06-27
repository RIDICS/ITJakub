using System.Collections.Generic;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchKeyWordsViewModel
    {
        public List<SearchResultWithHtmlContext> Results { get; set; }
    }
}