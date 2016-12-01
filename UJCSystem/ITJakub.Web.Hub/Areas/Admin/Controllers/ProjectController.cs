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

        public IActionResult ProjectModuleTab(ProjectModuleTabType tabType)
        {
            switch (tabType)
            {
                case ProjectModuleTabType.ResourcePreview:
                    return PartialView("Resource/_Preview");
                case ProjectModuleTabType.ResourceDiscussion:
                    return PartialView("Resource/_Discussion");
                case ProjectModuleTabType.ResourceMetadata:
                    return PartialView("Resource/_Metadata");
                case ProjectModuleTabType.WorkPublications:
                    return PartialView("Work/_Publications");
                case ProjectModuleTabType.WorkPageList:
                    return PartialView("Work/_PageList");
                case ProjectModuleTabType.WorkCooperation:
                    return PartialView("Work/_Cooperation");
                case ProjectModuleTabType.WorkMetadata:
                    return PartialView("Work/_Metadata");
                case ProjectModuleTabType.WorkHistory:
                    return PartialView("Work/_History");
                default:
                    return NotFound();
            }
        }

        [HttpPost]
        public IActionResult UploadResource(UploadFileRequest request)
        {
            return Json(new {});
        }

        [HttpPost]
        public IActionResult UploadNewResourceVersion(UploadFileRequest request)
        {
            return Json(new { });
        }
    }
}
