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
        public List<string> Alphabet = new List<string> { "A", "B", "C", "Č", "D", "Ď", "E", "F", "G", "H", "CH", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "Ř", "S", "Š", "T", "Ť", "U", "V", "W", "X", "Y", "Z", "Ž"};
    }
}