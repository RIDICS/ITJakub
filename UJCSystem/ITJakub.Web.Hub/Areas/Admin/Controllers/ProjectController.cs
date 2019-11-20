using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ITJakub.Web.Hub.Areas.Admin.Controllers.Constants;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Areas.Admin.Models.Request;
using ITJakub.Web.Hub.Areas.Admin.Models.Response;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Constants;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Helpers;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using ITJakub.Web.Hub.Options;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [LimitedAccess(PortalType.CommunityPortal)]
    [Area("Admin")]
    public class ProjectController : BaseController
    {
        private readonly ILocalizationService m_localization;

        public ProjectController(ControllerDataProvider controllerDataProvider, ILocalizationService localization) : base(
            controllerDataProvider)
        {
            m_localization = localization;
        }

        public IActionResult List(string search, int start, int count = PageSizes.ProjectList, ViewType viewType = ViewType.Full,
            ProjectOwnerTypeContract projectOwnerType = ProjectOwnerTypeContract.MyProjects)
        {
            var client = GetProjectClient();
            var result = client.GetProjectList(start, count, GetDefaultProjectType(), projectOwnerType, search, true, true, true);
            var projectItems = Mapper.Map<List<ProjectItemViewModel>>(result.List);
            var listViewModel = new ListViewModel<ProjectItemViewModel>
            {
                TotalCount = result.TotalCount,
                List = projectItems,
                PageSize = count,
                Start = start,
                SearchQuery = search
            };
            var filterTypes = new List<ProjectOwnerTypeContract>
            {
                ProjectOwnerTypeContract.MyProjects,
                ProjectOwnerTypeContract.ForeignProjects,
                ProjectOwnerTypeContract.AllProjects,
            };
            var viewModel = new ProjectListViewModel
            {
                Projects = listViewModel,
                AvailableBookTypes = ProjectConstants.AvailableBookTypes,
                FilterTypes = filterTypes.Select(x =>
                    new SelectListItem(m_localization.Translate(x.ToString(), "Admin"), x.ToString(), x == projectOwnerType)).ToList()
            };

            switch (viewType)
            {
                case ViewType.Widget:
                    return PartialView("_ProjectListContent", listViewModel);
                case ViewType.Full:
                    return View(viewModel);
                default:
                    return View(viewModel);
            }
        }

        public IActionResult Project(long id)
        {
            var client = GetProjectClient();
            var result = client.GetProject(id);
            var viewModel = Mapper.Map<ProjectItemViewModel>(result);
            return View(viewModel);
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

            var search = string.Empty;
            var start = 0;
            
            switch (tabType)
            {
                case ProjectModuleTabType.WorkPublications:
                    var snapshotList = projectClient.GetSnapshotList(projectId.Value, start, PageSizes.SnapshotList, search);
                    var listModel = CreateListViewModel<SnapshotViewModel, SnapshotAggregatedInfoContract>(snapshotList, start, PageSizes.SnapshotList, search);
                    var model = new SnapshotListViewModel
                    {
                        ProjectId = projectId.Value,
                        ListWrapper = listModel,
                    };
                    return PartialView("Work/_Publications", model);
                case ProjectModuleTabType.WorkPageList:
                    var pages = projectClient.GetAllPageList(projectId.Value);
                    return PartialView("Work/_PageList", pages);
                case ProjectModuleTabType.WorkCooperation:
                    var result = projectClient.GetUserGroupsByProject(projectId.Value, start, PageSizes.CooperationList, search);
                    var cooperationViewModel = new ListViewModel<UserGroupContract>
                    {
                        TotalCount = result.TotalCount,
                        List = result.List,
                        PageSize = PageSizes.CooperationList,
                        Start = start,
                        SearchQuery = search
                    };
                    
                    return PartialView("Work/_Cooperation", cooperationViewModel);
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
                case ProjectModuleTabType.WorkChapters:
                    var chapterList = projectClient.GetChapterList(projectId.Value);
                    var pageList = projectClient.GetAllPageList(projectId.Value);
                    var chapterEditorViewModel = new ChapterEditorViewModel
                    {
                        Chapters = Mapper.Map<List<ChapterHierarchyViewModel>>(chapterList),
                        Pages = Mapper.Map<List<PageViewModel>>(pageList)
                    };
                    return PartialView("Work/_ChapterEditor", chapterEditorViewModel);
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

        public IActionResult GetImageViewer(long projectId)
        {
            var client = GetProjectClient();
            var pages = client.GetAllPagesWithImageInfoList(projectId);
            return PartialView("Resource/_Images", pages);
        }
        
        public IActionResult ImagesPageList(long projectId)
        {
            var client = GetProjectClient();
            var pages = client.GetAllPagesWithImageInfoList(projectId);
            return PartialView("Resource/SubView/_PageWithImagesTable", pages);
        }

        public IActionResult GetTextPreview()
        {
            return PartialView("Resource/_Preview");
        }

        public IActionResult GetTermsEditor(long projectId)
        {
            var client = GetProjectClient();
            var pages = client.GetAllPageList(projectId);
            
            var termClient = GetTermClient();
            var termCategories = termClient.GetTermCategoriesWithTerms();
            
            return PartialView("Resource/_Terms",  new TermEditorViewModel
            {
                TermCategories = termCategories,
                Pages = pages,
            });
        }

        public IActionResult PageList(long projectId)
        {
            var client = GetProjectClient();
            var pages = client.GetAllPageList(projectId);
            return PartialView("Work/Subview/_PageTable", pages);
        }

        public IActionResult ChapterList(long projectId)
        {
            var projectClient = GetProjectClient();
            var chapterList = projectClient.GetChapterList(projectId);
            var pageList = projectClient.GetAllPageList(projectId);
            var chapterEditorViewModel = new ChapterEditorViewModel
            {
                Chapters = Mapper.Map<List<ChapterHierarchyViewModel>>(chapterList),
                Pages = Mapper.Map<List<PageViewModel>>(pageList)
            };
            
            return PartialView("Work/SubView/_ChapterTable", chapterEditorViewModel);
        }
        
        public IActionResult SnapshotList(long projectId, string search, int start, int count = PageSizes.SnapshotList)
        {
            var client = GetProjectClient();

            search = search ?? string.Empty;
            var snapshotList = client.GetSnapshotList(projectId, start, count, search);
            var model = CreateListViewModel<SnapshotViewModel, SnapshotAggregatedInfoContract>(snapshotList, start, count, search);

            return PartialView("Work/SubView/_PublicationListPage", model);
        } 
        
        public IActionResult CooperationList(long projectId, string search, int start, int count = PageSizes.CooperationList)
        {
            var client = GetProjectClient();

            search = search ?? string.Empty;
            var result = client.GetUserGroupsByProject(projectId, start, count, search);
            var viewModel = new ListViewModel<UserGroupContract>
            {
                TotalCount = result.TotalCount,
                List = result.List,
                PageSize = count,
                Start = start,
                SearchQuery = search
            };
            return PartialView("Work/SubView/_CooperationList", viewModel);
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

            var newProject = new CreateProjectContract
            {
                Name = request.Name,
                ProjectType = GetDefaultProjectType(),
                BookTypesForForum = request.SelectedBookTypes,
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
        public IActionResult RenameProject([FromBody] RenameProjectRequest request)
        {
            var client = GetProjectClient();

            client.UpdateProject(request.Id, new ItemNameContract
            {
                Name = request.NewProjectName,
            });
            return AjaxOkResponse();
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
                LastModificationText = DateTime.Now.ToString(m_localization.GetRequestCulture()),
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