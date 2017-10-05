using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Areas.Admin.Models.Request;
using ITJakub.Web.Hub.Areas.Admin.Models.Response;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
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
            using (var client = GetRestClient())
            {
                const int start = 0;
                var result = client.GetProjectList(start, ProjectListPageSize);
                var viewModel = CreateProjectListViewModel(result, start);
                return View(viewModel);
            }
        }

        public IActionResult Project(long id)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetProject(id);
                var viewModel = Mapper.Map<ProjectItemViewModel>(result);
                return View(viewModel);
            }
        }

        public IActionResult ProjectListContent(int start, int count)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetProjectList(start, count);
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

            using (var client = GetRestClient())
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
                        var literaryKinds = client.GetLiteraryKindList();
                        var literaryGenres = client.GetLitararyGenreList();
                        var literaryOriginals = client.GetLitararyOriginalList();
                        var responsibleTypes = client.GetResponsibleTypeList();
                        var projectMetadata = client.GetProjectMetadata(projectId.Value, true, true, true, true, true);
                        var workMetadaViewModel = Mapper.Map<ProjectWorkMetadataViewModel>(projectMetadata);
                        workMetadaViewModel.AllLiteraryKindList = literaryKinds;
                        workMetadaViewModel.AllLiteraryGenreList = literaryGenres;
                        workMetadaViewModel.AllLiteraryOriginalList = literaryOriginals;
                        workMetadaViewModel.AllResponsibleTypeList =
                            Mapper.Map<List<ResponsibleTypeViewModel>>(responsibleTypes);
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

            using (var client = GetRestClient())
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
            using (var client = GetRestClient())
            {
                var resourceVersionList = client.GetResourceVersionHistory(resourceId);
                var viewModel = Mapper.Map<List<ResourceVersionViewModel>>(resourceVersionList);
                return PartialView("_ResourceVersion", viewModel);
            }
        }

        public IActionResult NewSnapshot(long projectId)
        {
            using (var client = GetRestClient())
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
            using (var client = GetRestClient())
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
            using (var client = GetRestClient())
            {
                client.DeleteProject(request.Id);
                return Json(new { });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadResource()
        {
            var boundary = UploadHelper.GetBoundary(Request.ContentType);
            var reader = new MultipartReader(boundary, Request.Body, UploadHelper.MultipartReaderBufferSize);

            var valuesByKey = new Dictionary<string, string>();
            MultipartSection section;

            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                var contentDispo = section.GetContentDispositionHeader();

                if (contentDispo.IsFileDisposition())
                {
                    if (!valuesByKey.TryGetValue("sessionId", out var sessionId))
                    {
                        return BadRequest();
                    }

                    var fileSection = section.AsFileSection();

                    using (var client = GetRestClient())
                    {
                        client.UploadResource(sessionId, fileSection.FileStream, fileSection.FileName);
                    }
                }
                else if (contentDispo.IsFormDisposition())
                {
                    var formSection = section.AsFormDataSection();
                    var value = await formSection.GetValueAsync();
                    valuesByKey.Add(formSection.Name, value);
                }
            }

            return Json(new { });
        }

        [HttpPost]
        public async Task<IActionResult> UploadNewResourceVersion()
        {
            var boundary = UploadHelper.GetBoundary(Request.ContentType);
            var reader = new MultipartReader(boundary, Request.Body, UploadHelper.MultipartReaderBufferSize);

            var valuesByKey = new Dictionary<string, string>();
            MultipartSection section;

            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                var contentDispo = section.GetContentDispositionHeader();

                if (contentDispo.IsFileDisposition())
                {
                    if (!valuesByKey.TryGetValue("sessionId", out var sessionId))
                    {
                        return BadRequest();
                    }

                    var fileSection = section.AsFileSection();

                    using (var client = GetRestClient())
                    {
                        client.UploadResource(sessionId, fileSection.FileStream, fileSection.FileName);
                    }
                }
                else if (contentDispo.IsFormDisposition())
                {
                    var formSection = section.AsFormDataSection();
                    var value = await formSection.GetValueAsync();
                    valuesByKey.Add(formSection.Name, value);
                }
            }

            return Json(new { });
        }

        public IActionResult GetResourceList(long projectId, ResourceTypeEnumContract resourceType)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetResourceList(projectId, resourceType);
                return Json(result);
            }
        }

        [HttpPost]
        public IActionResult ProcessUploadedResources([FromBody] ProcessResourcesRequest request)
        {
            using (var client = GetRestClient())
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
            using (var client = GetRestClient())
            {
                var resourceVersionId = client.ProcessUploadedResourceVersion(request.ResourceId,
                    new NewResourceContract
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
            using (var client = GetRestClient())
            {
                client.DeleteResource(request.ResourceId);
                return Json(new { });
            }
        }

        [HttpPost]
        public IActionResult RenameResource([FromBody] RenameResourceRequest request)
        {
            using (var client = GetRestClient())
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
            using (var client = GetRestClient())
            {
                var newResourceId = client.DuplicateResource(request.ResourceId);
                return Json(newResourceId);
            }
        }

        [HttpPost]
        public IActionResult CreateLiteraryKind([FromBody] LiteraryKindContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateLiteraryKind(request);
                return Json(newId);
            }
        }

        [HttpPost]
        public IActionResult CreateLiteraryGenre([FromBody] LiteraryGenreContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateLiteraryGenre(request);
                return Json(newId);
            }
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] OriginalAuthorContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateOriginalAuthor(request);
                return Json(newId);
            }
        }

        [HttpPost]
        public IActionResult CreateResponsiblePerson([FromBody] ResponsiblePersonContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateResponsiblePerson(request);
                return Json(newId);
            }
        }

        [HttpPost]
        public IActionResult CreateResponsibleType([FromBody] ResponsibleTypeContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateResponsibleType(request);
                return Json(newId);
            }
        }

        [HttpPost]
        public IActionResult CreateCategory([FromBody] CategoryContract request)
        {
            using (var client = GetRestClient())
            {
                var newId = client.CreateCategory(request);
                return Json(newId);
            }
        }

        public IActionResult GetCategoryList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetCategoryList();
                return Json(result);
            }
        }

        public IActionResult GetResponsibleTypeList()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetResponsibleTypeList();
                return Json(result);
            }
        }

        [HttpPost]
        public IActionResult SaveMetadata([FromQuery] long projectId, [FromBody] SaveMetadataRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            using (var client = GetRestClient())
            {
                var contract = new ProjectMetadataContract
                {
                    Authors = request.Authors,
                    BiblText = request.BiblText,
                    Copyright = request.Copyright,
                    ManuscriptCountry = request.ManuscriptCountry,
                    ManuscriptExtent = request.ManuscriptExtent,
                    ManuscriptIdno = request.ManuscriptIdno,
                    ManuscriptRepository = request.ManuscriptRepository,
                    ManuscriptSettlement = request.ManuscriptSettlement,
                    NotAfter = request.NotAfter != null ? (DateTime?) new DateTime(request.NotAfter.Value, 1, 1) : null,
                    NotBefore = request.NotBefore != null
                        ? (DateTime?) new DateTime(request.NotBefore.Value, 1, 1)
                        : null,
                    OriginDate = request.OriginDate,
                    PublishDate = request.PublishDate,
                    PublishPlace = request.PublishPlace,
                    PublisherText = request.PublisherText,
                    PublisherEmail = request.PublisherEmail,
                    RelicAbbreviation = request.RelicAbbreviation,
                    SourceAbbreviation = request.SourceAbbreviation,
                    SubTitle = request.SubTitle,
                    Title = request.Title
                };
                long newResourceVersionId = -1;
                int unsuccessRequestCount = 0;

                try
                {
                    newResourceVersionId = client.CreateNewProjectMetadataVersion(projectId, contract);
                }
                catch (HttpRequestException)
                {
                    unsuccessRequestCount++;
                }

                try
                {
                    client.SetProjectAuthors(projectId, new IntegerIdListContract {IdList = request.AuthorIdList});
                }
                catch (HttpRequestException)
                {
                    unsuccessRequestCount++;
                }

                try
                {
                    client.SetProjectResponsiblePersons(projectId,
                        new IntegerIdListContract {IdList = request.ResponsiblePersonIdList});
                }
                catch (HttpRequestException)
                {
                    unsuccessRequestCount++;
                }

                try
                {
                    client.SetProjectLiteraryKinds(projectId,
                        new IntegerIdListContract {IdList = request.LiteraryKindIdList});
                }
                catch (HttpRequestException)
                {
                    unsuccessRequestCount++;
                }

                try
                {
                    client.SetProjectLiteraryGenres(projectId,
                        new IntegerIdListContract {IdList = request.LiteraryGenreIdList});
                }
                catch (HttpRequestException)
                {
                    unsuccessRequestCount++;
                }

                if (unsuccessRequestCount > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var response = new SaveMetadataResponse
                {
                    NewResourceVersionId = newResourceVersionId,
                    LastModificationText = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                    LiteraryOriginalText =
                        LiteraryOriginalTextConverter.GetLiteraryOriginalText(request.ManuscriptCountry,
                            request.ManuscriptSettlement,
                            request.ManuscriptRepository, request.ManuscriptIdno, request.ManuscriptExtent),
                };
                return Json(response);
            }
        }

        [HttpPost]
        public IActionResult SaveComment(string jsonBody)
        {
            if (jsonBody == null)
            {
                return new JsonResult("Error");
            }
            dynamic json = JsonConvert.DeserializeObject(jsonBody);
            var page = json.page;
            var filename = page + "-" + Guid.NewGuid();
            using (var newCommentFile = new FileStream($".\\comments\\{filename}.txt", FileMode.Create,
                FileAccess.ReadWrite))
            {
                using (var writer = new StreamWriter(newCommentFile))
                {
                    writer.Write(jsonBody);
                }
            }
            return new JsonResult("Written");
        }

        [HttpPost]
        public IActionResult GetGuid()
        {
            var guid = Guid.NewGuid();
            return new JsonResult(guid);
        }

        [HttpPost]
        public IActionResult LoadCommentFile(int pageNumber)
        {
            var i = 0;
            string[] parts;
            try
            {
                const string path = @".\\comments\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string[] files = Directory.GetFiles(path, pageNumber + "-*.txt");
                parts = new string[files.Length];
                foreach (string file in files)
                {
                    using (var textFile = new FileStream(file,
                        FileMode.Open,
                        FileAccess.Read))
                    {
                        using (var reader = new StreamReader(textFile))
                        {
                            var text = reader.ReadToEnd();
                            parts[i] = text;
                            i++;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                string[] response = {"error-no-file"};
                return new JsonResult(response); //TODO handle exceptions properly
            }
            if (parts.Length > 0)
            {
                return new JsonResult(parts);
            }
            else
            {
                string[] response = {"error-no-file"};
                return new JsonResult(response); //TODO handle exceptions properly
            }
        }

        [HttpPost]
        public IActionResult LoadRenderedCompositionPage(int pageNumber) //TODO add logic
        {
            string parts;
            if (pageNumber == 1)
            {
                parts =
                    "<span id=\"80a055fb-8d6a-4b61-ac63-4e0c845425c2-text\">Comment to text</span><span id=\"4b877225-9456-403d-86a5-c7f8901572a5-text\">Cras sit amet nibh libero,</span><span> in gravida nulla. <span id=\"1504620630028-text\">Nulla</span> vel metus scelerisque ante sollicitudin commodo. Cras purus odio, vestibulum in vulputate at, tempus viverra turpis.</span>";
            }
            else if (pageNumber == 2)
            {
                parts =
                    "<span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.<br>Nullam vitae <span id=\"3844fc54-83cb-49f5-a6c9-63c55138bfb4-text\">posuere lectus</span>.<br>Vivamus vitae tincidunt eros, sit amet euismod lectus.<br>Donec in lorem venenatis, faucibus ligula faucibus, condimentum purus.</span>";
            }
            else if (pageNumber == 3)
            {
                parts =
                    "<span>Lorem ipsum dolor sit amet, consectetur adipiscing elit.<br>Nullam vitae </span><span id=\"cd8bcd2b-cf57-4a8f-8f9b-3449c20b597d-text\">posuere lectus</span>.<br><span>Vivamus vitae tincidunt eros, sit amet euismod lectus.<br>Donec in lorem venenatis, faucibus ligula faucibus, condimentum purus.</span>";
            }
            else
                parts =
                    "<span>But I must explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give you a complete account of the system, and expound the actual teachings of the great explorer of the truth, the master-builder of human happiness. No one rejects, dislikes, or avoids pleasure itself, because it is pleasure, but because those who do not know how to pursue pleasure rationally encounter consequences that are extremely painful. Nor again is there anyone who loves or pursues or desires to obtain pain of itself, because it is pain, but because occasionally circumstances occur in which toil and pain can procure him some great pleasure.</span>";

            return new JsonResult(parts); //TODO handle exceptions, return "error-no-file"
        }

        [HttpPost]
        public IActionResult LoadPlaintextCompositionPage(int pageNumber) //TODO add logic
        {
            string parts;
            if (pageNumber == 2)
            {
                parts =
                    "$3844fc54-83cb-49f5-a6c9-63c55138bfb4%HIIIIIIIIIIII\nHHHHHHHHHHHHHHHHH\n%3844fc54-83cb-49f5-a6c9-63c55138bfb4$";
            }
            else
                parts =
                    "Plain text\nAAAAAAAAAAAAAAAAAAAAAAA\nBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB\nCCCCCCCCCCCCCCCCCCCCCCCCCCC";

            return new JsonResult(parts); //TODO handle exceptions, return "error-no-file"
        }

        [HttpPost]
        public IActionResult GetNumberOfPages(string compositionId) //TODO add logic
        {
            int pages=2000;

            return new JsonResult(pages); //TODO handle exceptions, return "error-no-file"
        }

        #region Typeahead

        public IActionResult GetTypeaheadOriginalAuthor(string query)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetOriginalAuthorAutocomplete(query);
                return Json(result);
            }
        }

        public IActionResult GetTypeaheadResponsiblePerson(string query)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetResponsiblePersonAutocomplete(query);
                return Json(result);
            }
        }

        #endregion
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