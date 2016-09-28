using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers.Plugins.Bibliography
{
    public class BibliographyController : Controller
    {
        // GET: Bibliography
        public ActionResult GetConfiguration()
        {
            string fullPath = "~/content/BibliographyConfiguration/configuration.json";
            return File(fullPath, "application/json", fullPath);
        }
    }
}