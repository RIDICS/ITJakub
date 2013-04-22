using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ujc.Naki.MVCWebLayer.Services;
using Ujc.Naki.MVCWebLayer.Services.Enums;

namespace Ujc.Naki.MVCWebLayer.ViewModels
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }
        public string[] SearchPart { get; set; }
        public Dictionary<BookCategory, List<string>> SearchedBooks { get; set; }
    }
}