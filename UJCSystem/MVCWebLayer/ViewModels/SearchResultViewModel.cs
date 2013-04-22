using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITJakub.Contracts.Searching;
using Ujc.Naki.MVCWebLayer.Services;
using Ujc.Naki.MVCWebLayer.Services.Enums;

namespace Ujc.Naki.MVCWebLayer.ViewModels
{
    public class SearchResultViewModel
    {
        public SearchViewModel Search { get; set; }
        public string[] FoundWords { get; set; }
        public Dictionary<BookCategory, List<string>> FoundByType { get; set; }
    }

    public class SearchKeyWordsViewModel
    {
        public KwicResult[] Results { get; set; }
    }
}