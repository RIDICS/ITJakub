using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models.Requests;
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

        public IActionResult ProjectModule(long id, ProjectModuleType moduleType)
        {
            switch (moduleType)
            {
                case ProjectModuleType.Work:
                    return PartialView("_ProjectWork");
                case ProjectModuleType.Resource:
                    return PartialView("_ProjectResource");
                default:
                    return PartialView("_ProjectWork");
            }
        }

        [HttpPost]
        public IActionResult UploadResource(UploadFileRequest request)
        {
            return Json(new {});
        }
    }
}
