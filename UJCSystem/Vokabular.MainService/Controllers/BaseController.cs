using System.IO;
using Microsoft.AspNetCore.Mvc;
using Vokabular.RestClient.Headers;

namespace Vokabular.MainService.Controllers
{
    public abstract class BaseController : Controller
    {
        protected void SetTotalCountHeader(int totalCount)
        {
            Response.Headers.Add(CustomHttpHeaders.TotalCount, totalCount.ToString());
        }

        protected FileStreamResult File(Stream fileStream, string contentType, string fileDownloadName, long? fileSize)
        {
            Response.ContentLength = fileSize;
            return base.File(fileStream, contentType, fileDownloadName);
        }
    }
}
