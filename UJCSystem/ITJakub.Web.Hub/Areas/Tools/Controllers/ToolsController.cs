using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Tools.Controllers
{
    [RouteArea("Tools")]
    public class ToolsController : Controller
    {

        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

    }
}