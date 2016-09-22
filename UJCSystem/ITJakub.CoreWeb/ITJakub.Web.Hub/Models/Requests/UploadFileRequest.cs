using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ITJakub.Web.Hub.Models.Requests
{
    public class UploadFileRequest
    {
        public string SessionId { get; set; }

        public IList<IFormFile> Files { get; set; }
    }
}