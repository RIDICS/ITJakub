using System;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ProjectItemViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastEditDate { get; set; }
        public string CreateUser { get; set; }
        public string LastEditUser { get; set; }
        public string PublisherText { get; set; }
        public string LiteraryOriginalText { get; set; }
        public int PageCount { get; set; }
    }
}
