using Microsoft.AspNetCore.Http;

namespace ITJakub.Web.Hub.Areas.Admin.Models.Request
{
    public class SaveImageResourceRequest
    {
        public long? PageId { get; set; }
        public long? ImageId { get; set; }
        public long? ResourceVersionId { get; set; }
        public IFormFile File { get; set; }
        public string Comment { get; set; }
    }
}
