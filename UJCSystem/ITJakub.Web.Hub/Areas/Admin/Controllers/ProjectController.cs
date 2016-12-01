using System;
using System.Collections.Generic;
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
                    var resourceMetadataViewModel = ProjectMock.GetProjectResourceMetadata();
                    return PartialView("Resource/_Metadata", resourceMetadataViewModel);
                case ProjectModuleTabType.WorkPublications:
                    var publicationsViewModel = new List<SnapshotViewModel>
                    {
                        ProjectMock.GetSnapshot(),
                        ProjectMock.GetSnapshot()
                    };
                    return PartialView("Work/_Publications", publicationsViewModel);
                case ProjectModuleTabType.WorkPageList:
                    return PartialView("Work/_PageList");
                case ProjectModuleTabType.WorkCooperation:
                    return PartialView("Work/_Cooperation");
                case ProjectModuleTabType.WorkMetadata:
                    var workMetadaViewModel = ProjectMock.GetProjectWorkMetadata();
                    return PartialView("Work/_Metadata", workMetadaViewModel);
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

    public static class ProjectMock
    {
        public static SnapshotViewModel GetSnapshot()
        {
            return new SnapshotViewModel
            {
                Id = 5,
                PublishDate = DateTime.Now,
                TextResourceCount = 3,
                PublishedTextResourceCount = 3,
                ImageResourceCount = 30,
                PublishedImageResourceCount = 30,
                AudioResourceCount = 1,
                PublishedAudioResourceCount = 1,
                Author = "Jan Novák"
            };
        }

        public static ProjectWorkMetadataViewModel GetProjectWorkMetadata()
        {
            return new ProjectWorkMetadataViewModel
            {
                Editor = "Jan Novák",
                LastModification = DateTime.Now,
                LiteraryGenre = "xxxxxxx",
                LiteraryKind = "xxxxxxx",
                LiteraryOriginal = "xxxxxxx",
                RelicAbbreviation = "xxxxxxx",
                SourceAbbreviation = "xxxxxxx"
            };
        }

        public static ProjectResourceMetadataViewModel GetProjectResourceMetadata()
        {
            return new ProjectResourceMetadataViewModel
            {
                Editor = "Jan Novák",
                Editor2 = "Josef Novák",
                LastModification = DateTime.Now,
                EditionNote = "xxxxxxx"
            };
        }
    }
}
