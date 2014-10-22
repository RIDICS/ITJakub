using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers.Plugins.Bibliography
{
    public class BibliographyController : Controller
    {
        // GET: Bibliography
        public ActionResult GetConfiguration()
        {

            return Json(new { configuration = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}