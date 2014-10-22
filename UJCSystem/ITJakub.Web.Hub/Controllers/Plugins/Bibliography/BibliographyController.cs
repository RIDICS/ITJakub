using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers.Plugins.Bibliography
{
    public class BibliographyController : Controller
    {
        // GET: Bibliography
        public ActionResult GetConfiguration()
        {
            string fullPath = Server.MapPath("~/Content/Plugins/Bibliography/configuration.json");
            return File(fullPath, "application/json", fullPath);
        }
    }
}