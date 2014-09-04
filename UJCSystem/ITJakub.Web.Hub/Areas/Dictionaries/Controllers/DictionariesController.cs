using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Dictionaries.Controllers
{
    [RouteArea("Dictionaries")]
    public class DictionariesController : Controller
    {
        // GET: Dictionaries/Dictionaries
        public ActionResult Index()
        {
            return View();
        }
    }
}