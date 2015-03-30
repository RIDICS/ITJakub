using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.OldGrammar.Controllers
{
    [RouteArea("OldGrammar")]
    public class OldGrammarController : Controller
    {

        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
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