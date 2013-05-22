using System.Web.Mvc;

namespace ITJakub.MVCWebLayer.Controllers
{
    public class ModulesController : Controller
    {
        [HttpGet]
        public virtual ActionResult Index(string id)
        {
            return View();
        }
    }
}
