using System.Collections.Generic;
using System.Web.Mvc;
using ITJakub.Shared.Contracts;
using ITJakub.Web.Hub.Controllers.Plugins.Search;

namespace ITJakub.Web.Hub.Areas.Bibliographies.Controllers
{
    [RouteArea("Bibliographies")]
    public class BibliographiesController : Controller
    {

        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult TermsOfUse()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            return View();
        }

        public ActionResult Search(string term)
        {
            List<SearchResultContract> listBooks = m_serviceClient.Search(term);
            return Json(new { books = listBooks }, JsonRequestBehavior.AllowGet);
        }
    }
}