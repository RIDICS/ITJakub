using System;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ResourceVersionViewModel
    {
        public long Id { get; set; }
        public int VersionNumber { get; set; }
        public string Comment { get; set; }
        public DateTime CreateDate { get; set; }
        public string Author { get; set; }
    }
}
