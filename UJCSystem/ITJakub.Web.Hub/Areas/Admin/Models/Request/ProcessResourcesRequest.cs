namespace ITJakub.Web.Hub.Areas.Admin.Models.Request
{
    public class ProcessResourcesRequest
    {
        public long ProjectId { get; set; }
        public string SessionId { get; set; }
        public string Comment { get; set; }
    }
}