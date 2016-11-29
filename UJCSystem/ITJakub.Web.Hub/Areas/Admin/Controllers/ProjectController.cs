using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Managers;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectController : BaseController
    {
        public ProjectController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult Project(long id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}
