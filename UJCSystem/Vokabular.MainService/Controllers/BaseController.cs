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
    }
}
