﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Areas.Admin.Models.Request;
using ITJakub.Web.Hub.Areas.Admin.Models.Response;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.AspNetCore.Helpers;
using ITJakub.Web.Hub.Options;
using Scalesoft.Localization.AspNetCore;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [LimitedAccess(PortalType.CommunityPortal)]
    [Area("Admin")]
    public class ProjectController : BaseController
    {
        private const int ProjectListPageSize = 5;
        private const int SnapshotListPageSize = 10;

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
            var result = client.GetProjectList(start, ProjectListPageSize, GetDefaultProjectType(), null, true);
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
            var result = client.GetProjectList(start, count, GetDefaultProjectType(), null, true);
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
                    var search = string.Empty;
                    var start = 0;
                    var snapshotList = projectClient.GetSnapshotList(projectId.Value, start, SnapshotListPageSize, search);
                    var model = CreateListViewModel<SnapshotViewModel, SnapshotAggregatedInfoContract>(snapshotList, start, SnapshotListPageSize, search);
                    return PartialView("Work/_Publications", model);
                case ProjectModuleTabType.WorkPageList:
                    var pages = projectClient.GetAllPageList(projectId.Value);
                    return PartialView("Work/_PageList", pages);
                case ProjectModuleTabType.WorkCooperation:
                    return PartialView("Work/_Cooperation");
                case ProjectModuleTabType.WorkMetadata:
                    var literaryOriginals = codeListClient.GetLiteraryOriginalList();
                    var responsibleTypes = codeListClient.GetResponsibleTypeList();
                    var projectMetadata = projectClient.GetProjectMetadata(projectId.Value, true, true, false, false, true, false, false);
                    var workMetadataViewModel = Mapper.Map<ProjectWorkMetadataViewModel>(projectMetadata);
                    workMetadataViewModel.AllLiteraryOriginalList = literaryOriginals;
                    workMetadataViewModel.AllResponsibleTypeList = Mapper.Map<List<ResponsibleTypeViewModel>>(responsibleTypes);
                    return PartialView("Work/_Metadata", workMetadataViewModel);
                case ProjectModuleTabType.WorkCategorization:
                    var literaryKinds = codeListClient.GetLiteraryKindList();
                    var literaryGenres = codeListClient.GetLiteraryGenreList();
                    var categories = codeListClient.GetCategoryList();
                    var projectCategorization = projectClient.GetProjectMetadata(projectId.Value, false, false, true, true, false, true, true);
                    var workCategorizationViewModel = Mapper.Map<ProjectWorkCategorizationViewModel>(projectCategorization);
                    workCategorizationViewModel.AllLiteraryKindList = literaryKinds;
                    workCategorizationViewModel.AllLiteraryGenreList = literaryGenres;
                    workCategorizationViewModel.AllCategoryList = categories;
                    return PartialView("Work/_Categorization", workCategorizationViewModel);
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

        public IActionResult SnapshotList(long projectId, string search, int start, int count = SnapshotListPageSize)
        {
            var client = GetProjectClient();

            search = search ?? string.Empty;
            var snapshotList = client.GetSnapshotList(projectId, start, count, search);
            var model = CreateListViewModel<SnapshotViewModel, SnapshotAggregatedInfoContract>(snapshotList, start, count, search);

            return PartialView("Work/SubView/_PublicationListPage", model);
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
            var unsuccessfulData = new List<string>();

            try
            {
                newResourceVersionId = client.CreateNewProjectMetadataVersion(projectId, contract);
            }
            catch (HttpRequestException)
            {
                unsuccessfulData.Add(m_localization.Translate("Metadata", "Admin"));
            }

            try
            {
                client.SetProjectAuthors(projectId, new IntegerIdListContract {IdList = request.AuthorIdList});
            }
            catch (HttpRequestException)
            {
                unsuccessfulData.Add(m_localization.Translate("Authors", "Admin"));
            }

            try
            {
                client.SetProjectResponsiblePersons(projectId, request.ProjectResponsiblePersonIdList);
            }
            catch (HttpRequestException)
            {
                unsuccessfulData.Add(m_localization.Translate("ResponsiblePeople", "Admin"));
            }

            if (unsuccessfulData.Count > 0)
            {
                return new JsonResult(unsuccessfulData)
                {
                    StatusCode = (int)HttpStatusCode.BadGateway
                };
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

        [HttpPost]
        public IActionResult SaveCategorization([FromQuery] long projectId, [FromBody] SaveCategorizationRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var client = GetProjectClient();
            var unsuccessfulData = new List<string>();

            try
            {
                client.SetProjectLiteraryKinds(projectId,
                    new IntegerIdListContract { IdList = request.LiteraryKindIdList });
            }
            catch (HttpRequestException)
            {
                unsuccessfulData.Add(m_localization.Translate("LiteraryKind", "Admin"));
            }

            try
            {
                client.SetProjectCategories(projectId,
                    new IntegerIdListContract { IdList = request.CategoryIdList });
            }
            catch (HttpRequestException)
            {
                unsuccessfulData.Add(m_localization.Translate("Category", "Admin"));
            }

            try
            {
                client.SetProjectLiteraryGenres(projectId,
                    new IntegerIdListContract { IdList = request.LiteraryGenreIdList });
            }
            catch (HttpRequestException)
            {
                unsuccessfulData.Add(m_localization.Translate("LiteraryGenre", "Admin"));
            }

            try
            {
                client.SetProjectKeywords(projectId, new IntegerIdListContract { IdList = request.KeywordIdList });
            }
            catch (HttpRequestException)
            {
                unsuccessfulData.Add(m_localization.Translate("Keywords", "Admin"));
            }

            if (unsuccessfulData.Count > 0)
            {
                return new JsonResult(unsuccessfulData)
                {
                    StatusCode = (int)HttpStatusCode.BadGateway
                };
            }

            return AjaxOkResponse();
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