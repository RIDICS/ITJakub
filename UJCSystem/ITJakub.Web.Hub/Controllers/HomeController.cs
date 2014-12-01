using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}