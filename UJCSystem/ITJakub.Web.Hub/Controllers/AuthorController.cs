using System.Web.Helpers;
using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        public ActionResult GetAllAuthors()
        {
            var authors = m_serviceClient.GetAllAuthors();
            return Json(new { Authors = authors });
        }
    }
}