using System;
using System.Collections.Generic;
using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Areas.Admin.Models.Contract;
using ITJakub.Web.Hub.Areas.Admin.Models.Request;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;

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
            using (var client = GetServiceClient())
            {
                var result = client.GetProjectList();
                var viewModel = Mapper.Map<List<ProjectItemViewModel>>(result);
                return View(viewModel);
            }
        }

        public IActionResult Project(long id)
        {
            using (var client = GetServiceClient())
            {
                var result = client.GetProject(id);
                var viewModel = Mapper.Map<ProjectItemViewModel>(result);
                return View(viewModel);
            }
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

        public IActionResult ProjectResourceVersion()
        {
            var viewModel = new List<ResourceVersionViewModel>
            {
                ProjectMock.GetResourceVersion(1),
                ProjectMock.GetResourceVersion(2),
                ProjectMock.GetResourceVersion(3),
                ProjectMock.GetResourceVersion(4),
                ProjectMock.GetResourceVersion(5),
                ProjectMock.GetResourceVersion(6),
                ProjectMock.GetResourceVersion(7),
                ProjectMock.GetResourceVersion(8),
                ProjectMock.GetResourceVersion(9),
                ProjectMock.GetResourceVersion(10),
                ProjectMock.GetResourceVersion(11),
                ProjectMock.GetResourceVersion(12),
                ProjectMock.GetResourceVersion(13),
                ProjectMock.GetResourceVersion(14),
                ProjectMock.GetResourceVersion(15),
            };
            return PartialView("_ResourceVersion", viewModel);
        }

        public IActionResult NewSnapshot(long projectId)
        {
            var viewModel = ProjectMock.GetNewPulication();
            return PartialView("Work/_PublicationsNew", viewModel);
        }

        [HttpPost]
        public IActionResult CreateProject([FromBody] CreateProjectRequest request)
        {
            using (var client = GetServiceClient())
            {
                var newProject = new ProjectContract
                {
                    Name = request.Name
                };
                var newProjectId = client.CreateProject(newProject);
                return Json(newProjectId);
            }
        }

        [HttpPost]
        public IActionResult DeleteProject([FromBody] DeleteProjectRequest request)
        {
            using (var client = GetServiceClient())
            {
                client.DeleteProject(request.Id);
                return Json(new { });
            }
        }

        [HttpPost]
        public IActionResult UploadResource(UploadFileRequest request)
        {
            for (var i = 0; i < Request.Form.Files.Count; i++)
            {
                var file = Request.Form.Files[i];
                if (file != null && file.Length != 0)
                {
                    using (var client = GetServiceClient())
                    {
                        client.UploadResource(request.SessionId, file.OpenReadStream());
                    }
                }
            }
            return Json(new { });
        }

        [HttpPost]
        public IActionResult UploadNewResourceVersion(UploadFileRequest request)
        {
            for (var i = 0; i < Request.Form.Files.Count; i++)
            {
                var file = Request.Form.Files[i];
                if (file != null && file.Length != 0)
                {
                    using (var client = GetServiceClient())
                    {
                        client.UploadResource(request.SessionId, file.OpenReadStream());
                    }
                }
            }
            return Json(new { });
        }

        public IActionResult GetResourceList(long projectId, ProjectResourceType resourceType)
        {
            var result = new List<ProjectResourceContract>
            {
                ProjectMock.GetResource(1),
                ProjectMock.GetResource(2),
                ProjectMock.GetResource(3),
            };
            return Json(result);
        }

        [HttpPost]
        public IActionResult ProcessUploadedResources([FromBody] ProcessResourcesRequest request)
        {
            return Json(new { });
        }

        [HttpPost]
        public IActionResult ProcessUploadResourceVersion([FromBody] ProcessResourceVersionRequest request)
        {
            return Json(new { });
        }

        [HttpPost]
        public IActionResult DeleteResource([FromBody] DeleteResourceRequest request)
        {
            return Json(new { });
        }

        [HttpPost]
        public IActionResult RenameResource([FromBody] RenameResourceRequest request)
        {
            return Json(new { });
        }

        [HttpPost]
        public IActionResult DuplicateResource([FromBody] DuplicateResourceRequest request)
        {
            return Json(77);
        }
    }

    public static class ProjectMock
    {
        public static ProjectItemViewModel GetProjectItem()
        {
            return new ProjectItemViewModel
            {
                Id = 45,
                CreateDate = DateTime.Now.AddDays(-1),
                CreateUser = "Jan Novák",
                LastEditDate = DateTime.Now,
                LastEditUser = "Jan Novák",
                LiteraryOriginalText = "Praha, Národní knihovna České republiky, konec 14. století",
                Name = "Andělíku rozkochaný",
                PublisherText = "Praha, 2009–2015, oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.",
                PageCount = 1
            };
        }

        public static ProjectInfoViewModel GetProjectInfo(long id)
        {
            return new ProjectInfoViewModel
            {
                Id = id,
                Name = string.Format("Název projektu {0}", id)
            };
        }

        public static ProjectResourceContract GetResource(long id)
        {
            return new ProjectResourceContract
            {
                Id = id,
                Name = string.Format("Název {0}", id)
            };
        }

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
                LiteraryOriginal = new ProjectWorkLiteraryOriginalViewModel(),
                LiteraryOriginalText = "xxxxxxx",
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

        public static ResourceVersionViewModel GetResourceVersion(int versionNumber)
        {
            return new ResourceVersionViewModel
            {
                Id = versionNumber,
                Author = "Jan Novák",
                Comment = "První verze dokumentu [název díla]",
                CreateDate = DateTime.Now,
                VersionNumber = versionNumber
            };
        }

        public static NewPublicationViewModel GetNewPulication()
        {
            return new NewPublicationViewModel
            {
                ResourceList = new List<ResourceViewModel>
                {
                    GetResourceViewModel(1),
                    GetResourceViewModel(2),
                    GetResourceViewModel(3),
                    GetResourceViewModel(4),
                    GetResourceViewModel(5)
                },
                VisibilityForGroups = new List<GroupInfoViewModel>
                {
                    GetVisibilityForGroup(1),
                    GetVisibilityForGroup(2),
                    GetVisibilityForGroup(3),
                }
            };
        }

        private static GroupInfoViewModel GetVisibilityForGroup(int id)
        {
            return new GroupInfoViewModel
            {
                GroupId = id,
                Name = string.Format("Skupina {0}", id)
            };
        }

        private static ResourceViewModel GetResourceViewModel(int id)
        {
            return new ResourceViewModel
            {
                Id = id,
                Name = string.Format("Strana {0}", id),
                VersionList = new List<VersionNumberViewModel>
                {
                    GetVersionNumber(1),
                    GetVersionNumber(2),
                }
            };
        }

        private static VersionNumberViewModel GetVersionNumber(int id)
        {
            return new VersionNumberViewModel
            {
                ResourceVersionId = id,
                VersionNumber = id
            };
        }
    }
}
