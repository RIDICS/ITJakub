using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ujc.Naki.MVCWebLayer.Services;
using Ujc.Naki.MVCWebLayer.Enums;
using Ujc.Naki.MVCWebLayer.Services.DTOs;

namespace Ujc.Naki.MVCWebLayer.ViewModels
{
    public class ListSourcesViewModel
    {
        public SourcesViewMode ViewMode { get; set; }
        public IEnumerable<Source> FoundSources { get; set; }
    }
}