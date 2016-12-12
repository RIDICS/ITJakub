namespace ITJakub.Web.Hub.Areas.Admin.Models.Request
{
    public class ProcessResourceVersionRequest
    {
        public long ResourceId { get; set; }
        public string SessionId { get; set; }
        public string Comment { get; set; }
    }
}