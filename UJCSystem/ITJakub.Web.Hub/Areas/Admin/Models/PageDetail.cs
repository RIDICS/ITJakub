using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class PageDetailViewModel
    {
        public string Text { get; set; }

        public FileStreamResult Image { get; set; }
    }
}