using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ujc.Naki.ITJakub.WebGui.Models
{
    public class SearchModel
    {
        [Display(Name = "Author")]
        public string Author { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }
    }
}