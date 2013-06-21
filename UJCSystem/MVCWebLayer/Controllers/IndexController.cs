using System.Web.Mvc;
using ITJakub.MVCWebLayer.ViewModels;

namespace ITJakub.MVCWebLayer.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index()
        {
            return View(new SearchResultViewModel());
        }

    }
}
