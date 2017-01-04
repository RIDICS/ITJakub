using System.Collections.Generic;
using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Areas.Admin.Models.Request;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.MainService.DataContracts.Data;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectController : BaseController
    {
        private const int ProjectListPageSize = 5;

        public ProjectController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        private ProjectListViewModel CreateProjectListViewModel(ProjectListData data, int start)
        {
            var listViewModel = Mapper.Map<List<ProjectItemViewModel>>(data.List);
            return new ProjectListViewModel
            {
                TotalCount = data.TotalCount,
                List = listViewModel,
                PageSize = ProjectListPageSize,
                Start = start
            };
        }

        public IActionResult List()
        {
            using (var client = GetServiceClient())
            {
                const int start = 0;
                var result = client.GetProjectListFull(start, ProjectListPageSize);
                var viewModel = CreateProjectListViewModel(result, start);
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

        public IActionResult ProjectListContent(int start, int count)
        {
            using (var client = GetServiceClient())
            {
                var result = client.GetProjectListFull(start, count);
                var viewModel = CreateProjectListViewModel(result, start);
                return PartialView("_ProjectListContent", viewModel);
            }
        }

        public IActionResult ProjectModule(ProjectModuleType moduleType)
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

        public IActionResult ProjectWorkModuleTab(ProjectModuleTabType tabType, long? projectId)
        {
            if (projectId == null)
            {
                return BadRequest();
            }

            using (var client = GetServiceClient())
            {
                switch (tabType)
                {
                    case ProjectModuleTabType.WorkPublications:
                        var snapshotList = client.GetSnapshotList(projectId.Value);
                        var publicationsViewModel = Mapper.Map<List<SnapshotViewModel>>(snapshotList);
                        return PartialView("Work/_Publications", publicationsViewModel);
                    case ProjectModuleTabType.WorkPageList:
                        return PartialView("Work/_PageList");
                    case ProjectModuleTabType.WorkCooperation:
                        return PartialView("Work/_Cooperation");
                    case ProjectModuleTabType.WorkMetadata:
                        var projectMetadata = client.GetProjectMetadata(projectId.Value);
                        var workMetadaViewModel = Mapper.Map<ProjectWorkMetadataViewModel>(projectMetadata);
                        return PartialView("Work/_Metadata", workMetadaViewModel);
                    case ProjectModuleTabType.WorkHistory:
                        return PartialView("Work/_History");
                    default:
                        return NotFound();
                }
            }
        }

        public IActionResult ProjectResourceModuleTab(ProjectModuleTabType tabType, long? resourceId)
        {
            if (resourceId == null)
            {
                return BadRequest();
            }

            using (var client = GetServiceClient())
            {
                switch (tabType)
                {
                    case ProjectModuleTabType.ResourcePreview:
                        return PartialView("Resource/_Preview");
                    case ProjectModuleTabType.ResourceDiscussion:
                        return PartialView("Resource/_Discussion");
                    case ProjectModuleTabType.ResourceMetadata:
                        var resourceMetadata = client.GetResourceMetadata(resourceId.Value);
                        var resourceMetadataViewModel = Mapper.Map<ProjectResourceMetadataViewModel>(resourceMetadata);
                        return PartialView("Resource/_Metadata", resourceMetadataViewModel);
                    default:
                        return NotFound();
                }
            }
        }

        public IActionResult ProjectResourceVersion(long resourceId)
        {
            using (var client= GetServiceClient())
            {
                var resourceVersionList = client.GetResourceVersionHistory(resourceId);
                var viewModel = Mapper.Map<List<ResourceVersionViewModel>>(resourceVersionList);
                return PartialView("_ResourceVersion", viewModel);
            }
        }

        public IActionResult NewSnapshot(long projectId)
        {
            using (var client = GetServiceClient())
            {
                var resources = client.GetResourceList(projectId);
                // TODO
            }
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

        public IActionResult GetResourceList(long projectId, ResourceTypeContract resourceType)
        {
            using (var client = GetServiceClient())
            {
                var result = client.GetResourceList(projectId, resourceType);
                return Json(result);
            }
        }

        [HttpPost]
        public IActionResult ProcessUploadedResources([FromBody] ProcessResourcesRequest request)
        {
            using (var client = GetServiceClient())
            {
                var resourceId = client.ProcessUploadedResources(request.ProjectId, new NewResourceContract
                {
                    SessionId = request.SessionId,
                    Comment = request.Comment
                });
                return Json(resourceId);
            }
        }

        [HttpPost]
        public IActionResult ProcessUploadResourceVersion([FromBody] ProcessResourceVersionRequest request)
        {
            using (var client = GetServiceClient())
            {
                var resourceVersionId = client.ProcessUploadedResourceVersion(request.ResourceId, new NewResourceContract
                {
                    SessionId = request.SessionId,
                    Comment = request.Comment
                });
                return Json(resourceVersionId);
            }
        }

        [HttpPost]
        public IActionResult DeleteResource([FromBody] DeleteResourceRequest request)
        {
            using (var client = GetServiceClient())
            {
                client.DeleteResource(request.ResourceId);
                return Json(new { });
            }
        }

        [HttpPost]
        public IActionResult RenameResource([FromBody] RenameResourceRequest request)
        {
            using (var client = GetServiceClient())
            {
                client.RenameResource(request.ResourceId, new ResourceContract
                {
                    Name = request.NewName
                });
                return Json(new { });
            }
        }

        [HttpPost]
        public IActionResult DuplicateResource([FromBody] DuplicateResourceRequest request)
        {
            using (var client = GetServiceClient())
            {
                var newResourceId = client.DuplicateResource(request.ResourceId);
                return Json(newResourceId);
            }
        }
    }

    public static class ProjectMock
    {
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
