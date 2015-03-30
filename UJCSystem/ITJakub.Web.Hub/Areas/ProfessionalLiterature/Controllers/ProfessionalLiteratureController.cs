using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.ProfessionalLiterature.Controllers
{
    [RouteArea("ProfessionalLiterature")]
    public class ProfessionalLiteratureController : Controller
    {

        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        public ActionResult Index()
        {
            return View("Search");
        }

        public ActionResult Search()
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