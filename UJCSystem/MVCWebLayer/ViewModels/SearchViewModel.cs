using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITJakub.Contracts.Searching;
using Ujc.Naki.MVCWebLayer.Services;

namespace Ujc.Naki.MVCWebLayer.ViewModels
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }
        public string[] SearchPart { get; set; }
        public SearchResult[] SearchedBooks { get; set; }
    }
}