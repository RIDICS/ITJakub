using System.Web.Mvc;
using ITJakub.MVCWebLayer.ViewModels;

namespace ITJakub.MVCWebLayer.Controllers
{
    public class ModulesController : Controller
    {
        [HttpGet]
        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public virtual ActionResult Search(ModuleViewModel model)
        {
            return View("Search", null, new ModuleViewModel { 
                SearchTerm = model.SearchTerm,
                View = model.View
            });
        }
    }
}
