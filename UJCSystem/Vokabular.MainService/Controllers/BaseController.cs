using System.IO;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.RestClient.Contracts;
using Vokabular.RestClient.Headers;

namespace Vokabular.MainService.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        protected ObjectResult StatusCode(HttpStatusCode statusCode, object value)
        {
            return base.StatusCode((int) statusCode, value);
        }

        protected BadRequestObjectResult Error(string description)
        {
            return base.BadRequest(new ErrorContract
            {
                Description = description
            });
        }
    }
}
