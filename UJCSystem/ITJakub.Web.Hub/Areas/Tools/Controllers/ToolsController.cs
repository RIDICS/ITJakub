using System.Web.Mvc;
using ITJakub.Web.Hub.Models;

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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            var client = new ItJakubServiceClient();
            client.CreateAnonymousFeedback(model.Text, model.Name, model.Email);
            return View("Information");
        }

        public ActionResult List()
        {
            return View();
        }

    }
}