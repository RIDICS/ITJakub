using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class TextController : Controller
    {
        public ActionResult Editor()
        {
            return View("TextEditor");
        }
    }
}