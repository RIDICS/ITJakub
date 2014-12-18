using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Bibliographies.Controllers
{
    [RouteArea("Bibliographies")]
    public class BibliographiesController : Controller
    {
        // GET: Dictionaries/Dictionaries
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
    }
}