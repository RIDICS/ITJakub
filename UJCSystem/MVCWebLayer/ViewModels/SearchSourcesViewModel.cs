using System.Collections.Generic;
using ITJakub.MVCWebLayer.Services.DTOs;

namespace ITJakub.MVCWebLayer.ViewModels
{
    public class SearchSourcesViewModel
    {
        public string SearchTerm { get; set; }
        public IEnumerable<Source> FoundSources { get; set; }
    }
}