using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Areas.Admin.Models.Request;
using ITJakub.Web.Hub.Areas.Admin.Models.Response;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.AspNetCore.Helpers;
using ITJakub.Web.Hub.Options;
using Scalesoft.Localization.AspNetCore;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [LimitedAccess(PortalType.CommunityPortal)]
    [Area("Admin")]
    public class ProjectController : BaseController
    {
        private const int ProjectListPageSize = 5;
        private readonly ILocalizationService m_localization;

        public ProjectController(CommunicationProvider communicationProvider, ILocalizationService localization) : base(communicationProvider)
        {
            m_localization = localization;
        }

        private ProjectListViewModel CreateProjectListViewModel(PagedResultList<ProjectDetailContract> data, int start)
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
            var client = GetProjectClient();
            const int start = 0;
            var result = client.GetProjectList(start, ProjectListPageSize, null, true);
            var viewModel = CreateProjectListViewModel(result, start);
            return View(viewModel);
        }

        public IActionResult Project(long id)
        {
            var client = GetProjectClient();
            var result = client.GetProject(id);
            var viewModel = Mapper.Map<ProjectItemViewModel>(result);
            return View(viewModel);
        }

        public IActionResult ProjectListContent(int start, int count)
        {
            var client = GetProjectClient();
            var result = client.GetProjectList(start, count, null, true);
            var viewModel = CreateProjectListViewModel(result, start);
            return PartialView("_ProjectListContent", viewModel);
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

            var projectClient = GetProjectClient();
            var codeListClient = GetCodeListClient();

            switch (tabType)
            {
                case ProjectModuleTabType.WorkPublications:
                    var snapshotList = projectClient.GetSnapshotList(projectId.Value);
                    var publicationsViewModel = Mapper.Map<List<SnapshotViewModel>>(snapshotList);
                    return PartialView("Work/_Publications", publicationsViewModel);
                case ProjectModuleTabType.WorkPageList:
                    var pages = projectClient.GetAllPageList(projectId.Value);
                    return PartialView("Work/_PageList", pages);
                case ProjectModuleTabType.WorkCooperation:
                    return PartialView("Work/_Cooperation");
                case ProjectModuleTabType.WorkMetadata:
                    var literaryKinds = codeListClient.GetLiteraryKindList();
                    var literaryGenres = codeListClient.GetLiteraryGenreList();
                    var literaryOriginals = codeListClient.GetLiteraryOriginalList();
                    var responsibleTypes = codeListClient.GetResponsibleTypeList();
                    var categories = codeListClient.GetCategoryList();
                    var projectMetadata = projectClient.GetProjectMetadata(projectId.Value, true, true, true, true, true, true, true);
                    var workMetadaViewModel = Mapper.Map<ProjectWorkMetadataViewModel>(projectMetadata);
                    workMetadaViewModel.AllLiteraryKindList = literaryKinds;
                    workMetadaViewModel.AllLiteraryGenreList = literaryGenres;
                    workMetadaViewModel.AllLiteraryOriginalList = literaryOriginals;
                    workMetadaViewModel.AllCategoryList = categories;
                    workMetadaViewModel.AllResponsibleTypeList =
                        Mapper.Map<List<ResponsibleTypeViewModel>>(responsibleTypes);
                    return PartialView("Work/_Metadata", workMetadaViewModel);
                case ProjectModuleTabType.WorkHistory:
                    return PartialView("Work/_History");
                case ProjectModuleTabType.WorkNote:
                    return PartialView("Work/_Note");
                case ProjectModuleTabType.Forum:
                    var forum = projectClient.GetForum(projectId.Value);
                    ForumViewModel forumViewModel = null;
                    if (forum != null)
                    {
                        forumViewModel = Mapper.Map<ForumViewModel>(forum);
                    }

                    return PartialView("Work/_Forum", forumViewModel);
                default:
                    return NotFound();
            }
        }

        public IActionResult GetImageViewer()
        {
            return PartialView("Resource/_Images");
        }

        public IActionResult GetTextPreview()
        {
            return PartialView("Resource/_Preview");
        }

        public IActionResult ProjectResourceModuleTab(ProjectModuleTabType tabType, long? resourceId)
        {
            if (resourceId == null)
            {
                return BadRequest();
            }

            var client = GetResourceClient();

            switch (tabType)
            {
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

        public IActionResult ProjectResourceVersion(long resourceId)
        {
            var client = GetResourceClient();
            var resourceVersionList = client.GetResourceVersionHistory(resourceId);
            var viewModel = Mapper.Map<List<ResourceVersionViewModel>>(resourceVersionList);
            return PartialView("_ResourceVersion", viewModel);
        }

        public IActionResult NewSnapshot(long projectId)
        {
            var client = GetProjectClient();

            var audio = client.GetResourceList(projectId, ResourceTypeEnumContract.Audio);
            var images = client.GetResourceList(projectId, ResourceTypeEnumContract.Image);
            var text = client.GetResourceList(projectId, ResourceTypeEnumContract.Text);

            var model = new NewPublicationViewModel
            {
                ProjectId = projectId,
                Resources = new List<ResourcesViewModel>
                {
                    new ResourcesViewModel
                    {
                        ResourceList = Mapper.Map<IList<ResourceViewModel>>(text),
                        ResourceType = ResourceTypeEnumContract.Text,
                        Title = m_localization.Translate("TextSources", "Admin"),
                    },
                    new ResourcesViewModel
                    {
                        ResourceList = Mapper.Map<IList<ResourceViewModel>>(images),
                        ResourceType = ResourceTypeEnumContract.Image,
                        Title = m_localization.Translate("ImageSources", "Admin"),
                    },
                    new ResourcesViewModel
                    {
                        ResourceList = Mapper.Map<IList<ResourceViewModel>>(audio),
                        ResourceType = ResourceTypeEnumContract.Audio,
                        Title = m_localization.Translate("AudioSources", "Admin"),
                    }
                },
                AvailableBookTypes = new List<BookTypeEnumContract>
                {
                    BookTypeEnumContract.Edition,
                    BookTypeEnumContract.TextBank,
                    BookTypeEnumContract.Grammar,
                    BookTypeEnumContract.AudioBook
                }
            };

            model.PublishBookTypes = model.AvailableBookTypes.Select(x => new SelectableBookType{BookType = x}).ToList();

            return PartialView("Work/_PublicationsNew", model);
        }

        public IActionResult VersionList(long resourceId)
        {
            var client = GetResourceClient();
            var resourceVersionList = client.GetResourceVersionHistory(resourceId);
            var viewModel = Mapper.Map<List<ResourceVersionViewModel>>(resourceVersionList);
            return Json(viewModel);
        }
        
        [HttpPost]
        public IActionResult NewSnapshot(NewPublicationViewModel viewModel)
        {
            var client = GetProjectClient();

            var versionIds = new List<long>();
            foreach (var resource in viewModel.Resources)
            {
                if (resource.ResourceList != null)
                    versionIds.AddRange(resource.ResourceList.Where(x => x.IsSelected).Select(x => x.ResourceVersionId).ToList());
            }
            
            var createSnapshotContract = new CreateSnapshotContract
            {
                BookTypes = viewModel.PublishBookTypes.Where(x => x.IsSelected).Select(x => x.BookType).ToList(),
                DefaultBookType = viewModel.DefaultBookType,
                ResourceVersionIds = versionIds,
                Comment = viewModel.Comment
            };

            client.CreateSnapshot(viewModel.ProjectId, createSnapshotContract);
            return RedirectToAction("Project", new { id = viewModel.ProjectId });
        }


        public IActionResult GetText(long textId)
        {
            var client = GetProjectClient();
            var result = client.GetTextResourceVersion(textId, TextFormatEnumContract.Html);
            return Json(result);
        }

        public IActionResult GetImage(long imageId)
        {
            var client = GetProjectClient();
            var result = client.GetImageResourceVersion(imageId);
            return new FileStreamResult(result.Stream, result.MimeType);
        }

        public IActionResult GetRecordings(long trackId)
        {
            var client = GetProjectClient();
            var result = client.GetAudioTrackRecordings(trackId);
            return PartialView("Work/_AudioResource", result);
        }

        public FileResult DownloadAudio(long audioId)
        {
            var client = GetProjectClient();
            var fileData = client.GetAudio(audioId);
            Response.ContentLength = fileData.FileSize;
            return File(fileData.Stream, fileData.MimeType, fileData.FileName);
        }

        [HttpPost]
        public IActionResult CreateForum(long projectId)
        {
            var client = GetProjectClient();
            client.CreateForum(projectId);
            var forum = client.GetForum(projectId);
            var forumViewModel = Mapper.Map<ForumViewModel>(forum);
            return Json(forumViewModel);
        }

        [HttpPost]
        public IActionResult CreateProject([FromBody] CreateProjectRequest request)
        {
            var client = GetProjectClient();

            var newProject = new ProjectContract
            {
                Name = request.Name,
                ProjectType = GetDefaultProjectType(),
            };
            var newProjectId = client.CreateProject(newProject);
            return Json(newProjectId);
        }

        [HttpPost]
        public IActionResult DeleteProject([FromBody] DeleteProjectRequest request)
        {
            var client = GetProjectClient();

            client.DeleteProject(request.Id);
            return Json(new { });
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

                    var client = GetSessionClient();
                    client.UploadResource(sessionId, fileSection.FileStream, fileSection.FileName);
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

                    var client = GetSessionClient();
                    client.UploadResource(sessionId, fileSection.FileStream, fileSection.FileName);
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
            var client = GetProjectClient();
            var result = client.GetResourceList(projectId, resourceType);
            return Json(result);
        }

        [HttpPost]
        public IActionResult ProcessUploadedResources([FromBody] ProcessResourcesRequest request)
        {
            var client = GetProjectClient();
            var resourceId = client.ProcessUploadedResources(request.ProjectId, new NewResourceContract
            {
                SessionId = request.SessionId,
                Comment = request.Comment
            });
            return Json(resourceId);
        }

        [HttpPost]
        public IActionResult ProcessUploadResourceVersion([FromBody] ProcessResourceVersionRequest request)
        {
            var client = GetResourceClient();
            var resourceVersionId = client.ProcessUploadedResourceVersion(request.ResourceId,
                new NewResourceContract
                {
                    SessionId = request.SessionId,
                    Comment = request.Comment
                });
            return Json(resourceVersionId);
        }

        [HttpPost]
        public IActionResult CreateKeywordsWithArray(List<KeywordContract> request)
        {
            var client = GetCodeListClient();
            var ids = new List<int>();
            foreach (KeywordContract t in request)
            {
                var newId = client.CreateKeyword(t);
                ids.Add(newId);
            }

            return Json(ids);
        }

        [HttpGet]
        public IActionResult GetProjectsByAuthor(int authorId, int? start, int? count)
        {
            var client = GetCodeListClient();
            var result = client.GetProjectsByAuthor(authorId, start, count);
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetProjectsByResponsiblePerson(int responsiblePersonId, int? start, int? count)
        {
            var client = GetCodeListClient();
            var result = client.GetProjectsByResponsiblePerson(responsiblePersonId, start, count);
            return Json(result);
        }

        [HttpGet]
        public IActionResult KeywordTypeahead([FromQuery] string keyword, [FromQuery] int? count)
        {
            var client = GetCodeListClient();
            var result = client.GetKeywordAutocomplete(keyword, count);
            return Json(result);
        }

        [HttpPost]
        public IActionResult DeleteResource([FromBody] DeleteResourceRequest request)
        {
            var client = GetResourceClient();
            client.DeleteResource(request.ResourceId);
            return Json(new { });
        }

        [HttpPost]
        public IActionResult RenameResource([FromBody] RenameResourceRequest request)
        {
            var client = GetResourceClient();
            client.RenameResource(request.ResourceId, new ResourceContract
            {
                Name = request.NewName
            });
            return Json(new { });
        }

        [HttpPost]
        public IActionResult DuplicateResource([FromBody] DuplicateResourceRequest request)
        {
            var client = GetResourceClient();
            var newResourceId = client.DuplicateResource(request.ResourceId);
            return Json(newResourceId);
        }

        [HttpGet]
        public IActionResult GetProjectMetadata([FromQuery] long projectId, [FromQuery] bool includeAuthor,
            [FromQuery] bool includeResponsiblePerson,
            [FromQuery] bool includeKind, [FromQuery] bool includeGenre, [FromQuery] bool includeOriginal, [FromQuery] bool includeKeyword,
            [FromQuery] bool includeCategory)
        {
            var client = GetProjectClient();
            var response = client.GetProjectMetadata(projectId, includeAuthor,
                includeResponsiblePerson, includeKind, includeGenre, includeOriginal, includeKeyword, includeCategory);
            return Json(response);
        }

        [HttpPost]
        public IActionResult SaveMetadata([FromQuery] long projectId, [FromBody] SaveMetadataRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var client = GetProjectClient();

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
                Title = request.Title,
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
                client.SetProjectResponsiblePersons(projectId, request.ProjectResponsiblePersonIdList);
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
                client.SetProjectCategories(projectId,
                    new IntegerIdListContract {IdList = request.CategoryIdList});
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

            try
            {
                client.SetProjectKeywords(projectId, new IntegerIdListContract {IdList = request.KeywordIdList});
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


        #region Typeahead

        public IActionResult GetTypeaheadOriginalAuthor(string query)
        {
            var client = GetCodeListClient();
            var result = client.GetOriginalAuthorAutocomplete(query);
            return Json(result);
        }

        public IActionResult GetTypeaheadResponsiblePerson(string query)
        {
            var client = GetCodeListClient();
            var result = client.GetResponsiblePersonAutocomplete(query);
            return Json(result);
        }

        public IActionResult GetTypeaheadPublisher(string query)
        {
            var client = GetMetadataClient();
            var result = client.GetPublisherAutoComplete(query);
            return Json(result);
        }

        #endregion
    }
}