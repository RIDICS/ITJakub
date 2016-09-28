using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers.Plugins.Bibliography
{
    public class BibliographyController : Controller
    {
        private readonly IHostingEnvironment m_environment;

        public BibliographyController(IHostingEnvironment environment)
        {
            m_environment = environment;
        }

        // GET: Bibliography
        public ActionResult GetConfiguration()
        {
            string fullPath = Path.Combine(m_environment.ContentRootPath, "Content/BibliographyConfiguration/configuration.json");
            return File(fullPath, "application/json", fullPath);
        }
    }
}