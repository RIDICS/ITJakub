using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ujc.Naki.MVCWebLayer.Services;

namespace Ujc.Naki.MVCWebLayer.ViewModels
{
    public class SearchSourceDetailViewModel
    {
        public string SearchTerm { get; set; }
        public bool ShowResults { get; set; }
    }
}