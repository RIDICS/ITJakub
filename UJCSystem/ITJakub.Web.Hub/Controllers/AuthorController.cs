using System.Collections.Generic;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.ITJakubService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        public ActionResult GetAllAuthors()
        {
            IEnumerable<AuthorDetailContract> authors = m_serviceClient.GetAllAuthors();
            return Json(new { Authors = authors }, JsonRequestBehavior.AllowGet);
        }
    }
}