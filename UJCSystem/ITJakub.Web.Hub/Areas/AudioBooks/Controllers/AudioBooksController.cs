using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.AudioBooks.Controllers
{
    [RouteArea("AudioBooks")]
    public class AudioBooksController : Controller
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