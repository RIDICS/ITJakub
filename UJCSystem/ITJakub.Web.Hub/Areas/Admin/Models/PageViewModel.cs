namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class PageViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
    
    public class PageContentViewModel
    {
        public long PageId { get; set; }

        public string Text { get; set; }

        public bool HasImage { get; set; }
    }
}