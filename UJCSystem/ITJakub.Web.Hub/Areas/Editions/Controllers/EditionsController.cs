using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Editions.Controllers
{
    [RouteArea("Editions")]
    public class EditionsController : Controller
    {
        // GET: Editions/Editions
        public ActionResult Index()
        {
            return View();
        }
    }
}