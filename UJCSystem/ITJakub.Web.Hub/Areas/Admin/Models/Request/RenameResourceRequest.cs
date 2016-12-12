namespace ITJakub.Web.Hub.Areas.Admin.Models.Request
{
    public class RenameResourceRequest
    {
        public long ResourceId { get; set; }
        public string NewName { get; set; }
    }
}