using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Core.Structures.Shared;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Parameter;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : BaseController
    {
        private readonly ProjectManager m_projectManager;
        private readonly ProjectMetadataManager m_projectMetadataManager;
        private readonly ProjectInfoManager m_projectInfoManager;
        private readonly ForumSiteManager m_forumSiteManager;
        private readonly PermissionManager m_permissionManager;
        private readonly ProjectGroupManager m_projectGroupManager;
        private readonly AuthorizationManager m_authorizationManager;

        public ProjectController(ProjectManager projectManager, ProjectMetadataManager projectMetadataManager,
            ProjectInfoManager projectInfoManager, ForumSiteManager forumSiteManager, PermissionManager permissionManager,
            ProjectGroupManager projectGroupManager, AuthorizationManager authorizationManager)
        {
            m_projectManager = projectManager;
            m_projectMetadataManager = projectMetadataManager;
            m_projectInfoManager = projectInfoManager;
            m_forumSiteManager = forumSiteManager;
            m_permissionManager = permissionManager;
            m_projectGroupManager = projectGroupManager;
            m_authorizationManager = authorizationManager;
        }
        
        [HttpGet]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total records count")]
        public List<ProjectDetailContract> GetProjectList([FromQuery] int? start,
            [FromQuery] int? count,
            [FromQuery] ProjectTypeContract? projectType,
            [FromQuery] ProjectOwnerTypeContract? projectOwnerType,
            [FromQuery] string filterByName,
            [FromQuery] bool? fetchPageCount,
            [FromQuery] bool? fetchAuthors,
            [FromQuery] bool? fetchResponsiblePersons,
            [FromQuery] bool? fetchLatestChangedResource,
            [FromQuery] bool? fetchPermissions)
        {
            // Authorization is directly in SQL query

            var isFetchPageCount = fetchPageCount ?? false;
            var isFetchAuthors = fetchAuthors ?? false;
            var isFetchResponsiblePersons = fetchResponsiblePersons ?? false;
            var isFetchLatestChangedResource = fetchLatestChangedResource ?? false;
            var isFetchPermissions = fetchPermissions ?? false;
            var projectOwner = projectOwnerType ?? ProjectOwnerTypeContract.AllProjects;
            var result = m_projectManager.GetProjectList(start, count, projectType, projectOwner, filterByName, isFetchPageCount, isFetchAuthors, isFetchResponsiblePersons, isFetchLatestChangedResource, isFetchPermissions);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpGet("{projectId}")]
        [ProducesResponseType(typeof(ProjectDetailContract), StatusCodes.Status200OK)]
        public IActionResult GetProject(long projectId,
            [FromQuery] bool? fetchPageCount,
            [FromQuery] bool? fetchAuthors,
            [FromQuery] bool? fetchResponsiblePersons,
            //[FromQuery] bool? fetchLatestChangedResource,
            [FromQuery] bool? fetchPermissions)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            var isFetchPageCount = fetchPageCount ?? false;
            var isFetchAuthors = fetchAuthors ?? false;
            var isFetchResponsiblePersons = fetchResponsiblePersons ?? false;
            //var isFetchLatestChangedResource = fetchLatestChangedResource ?? false;
            var isFetchPermissions = fetchPermissions ?? false;

            var projectData = m_projectManager.GetProject(projectId, isFetchPageCount, isFetchAuthors, isFetchResponsiblePersons, isFetchPermissions);
            if (projectData == null)
                return NotFound();

            return Ok(projectData);
        }

        [HttpPost]
        [Authorize]
        public long CreateProject([FromBody] CreateProjectContract project)
        {
            // Authorization by book/resource/resource-version is not required

            return m_projectManager.CreateProject(project);
        }

        [HttpPut("{projectId}")]
        public void UpdateProject(long projectId, [FromBody] ItemNameContract data)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectManager.UpdateProject(projectId, data);
        }

        [HttpDelete("{projectId}")]
        public void RemoveProject(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.AdminProject);

            m_projectManager.RemoveProject(projectId);
        }

        [HttpGet("{projectId}/metadata")]
        [ProducesResponseType(typeof(ProjectMetadataResultContract), StatusCodes.Status200OK)]
        public IActionResult GetProjectMetadata(long projectId, [FromQuery] bool includeAuthor, [FromQuery] bool includeResponsiblePerson,
            [FromQuery] bool includeKind, [FromQuery] bool includeGenre, [FromQuery] bool includeOriginal, [FromQuery] bool includeKeyword, [FromQuery] bool includeCategory)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            var parameters = new GetProjectMetadataParameter
            {
                IncludeKind = includeKind,
                IncludeGenre = includeGenre,
                IncludeOriginal = includeOriginal,
                IncludeResponsiblePerson = includeResponsiblePerson,
                IncludeAuthor = includeAuthor,
                IncludeKeyword = includeKeyword,
                IncludeCategory = includeCategory
            };
            var resultData = m_projectMetadataManager.GetProjectMetadata(projectId, parameters);

            if (resultData == null)
                return NotFound();

            return Ok(resultData);
        }

        [HttpPost("{projectId}/metadata")]
        public long CreateNewProjectMetadataVersion(long projectId, [FromBody] ProjectMetadataContract metadata)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            return m_projectMetadataManager.CreateNewProjectMetadataVersion(projectId, metadata);
        }

        [HttpPut("{projectId}/literary-kind")]
        public void SetLiteraryKinds(long projectId, [FromBody] IntegerIdListContract kindIdList)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectInfoManager.SetLiteraryKinds(projectId, kindIdList);
        }

        [HttpPut("{projectId}/literary-genre")]
        public void SetLiteraryGenres(long projectId, [FromBody] IntegerIdListContract genreIdList)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectInfoManager.SetLiteraryGenres(projectId, genreIdList);
        }

        [HttpPut("{projectId}/literary-original")]
        public void SetLiteraryOriginal(long projectId, [FromBody] IntegerIdListContract litOriginalIdList)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectInfoManager.SetLiteraryOriginals(projectId, litOriginalIdList);
        }

        [HttpPut("{projectId}/keyword")]
        public void SetKeywords(long projectId, [FromBody] IntegerIdListContract keywordIdList)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectInfoManager.SetKeywords(projectId, keywordIdList);
        }

        [HttpPut("{projectId}/category")]
        public void SetCategories(long projectId, [FromBody] IntegerIdListContract categoryIdList)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectInfoManager.SetCategories(projectId, categoryIdList);
        }

        [HttpPut("{projectId}/author")]
        public void SetAuthors(long projectId, [FromBody] IntegerIdListContract authorIdList)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectInfoManager.SetAuthors(projectId, authorIdList);
        }

        [HttpPut("{projectId}/responsible-person")]
        public void SetResponsiblePersons(long projectId, [FromBody] List<ProjectResponsiblePersonIdContract> projectResposibleIdList)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectInfoManager.SetResponsiblePersons(projectId, projectResposibleIdList);
        }

        [HttpGet("{projectId}/literary-kind")]
        public List<LiteraryKindContract> GetLiteraryKinds(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            return m_projectInfoManager.GetLiteraryKinds(projectId);
        }

        [HttpGet("{projectId}/literary-genre")]
        public List<LiteraryGenreContract> GetLiteraryGenres(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            return m_projectInfoManager.GetLiteraryGenres(projectId);
        }

        [HttpGet("{projectId}/literary-original")]
        public List<LiteraryOriginalContract> GetLiteraryOriginal(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            return m_projectInfoManager.GetLiteraryOriginals(projectId);
        }

        [HttpGet("{projectId}/keyword")]
        public List<KeywordContract> GetKeywords(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            return m_projectInfoManager.GetKeywords(projectId);
        }

        [HttpGet("{projectId}/category")]
        public List<CategoryContract> GetCategories(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            return m_projectInfoManager.GetCategories(projectId);
        }

        [HttpGet("{projectId}/author")]
        public List<OriginalAuthorContract> GetAuthors(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            return m_projectInfoManager.GetAuthors(projectId);
        }

        [HttpGet("{projectId}/responsible-person")]
        public List<ProjectResponsiblePersonContract> GetProjectResponsiblePersons(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            return m_projectInfoManager.GetProjectResponsiblePersons(projectId);
        }

        [HttpGet("{projectId}/user-group")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total records count")]
        public List<UserGroupContract> GetUserGroupsByProject(long projectId, [FromQuery] int? start, [FromQuery] int? count,
            [FromQuery] string filterByName)
        {
            m_authorizationManager.AuthorizeBookOrPermission(projectId, PermissionFlag.ReadProject, PermissionNames.AssignPermissionsToRoles);

            var result = m_projectManager.GetUserGroupsByProject(projectId, start, count, filterByName);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpGet("{projectId}/forum")]
        [ProducesResponseType(typeof(ForumContract), StatusCodes.Status200OK)]
        public IActionResult GetForum(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            var forum = m_forumSiteManager.GetForum(projectId);
            return Ok(forum);
        }

        [HttpPost("{projectId}/forum")]
        public ActionResult<int> CreateForum(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            var forumId = m_forumSiteManager.CreateOrUpdateForums(projectId);

            return forumId != null ? (ActionResult<int>) Ok(forumId.Value) : BadRequest("Forum is disabled");
        }

        [HttpPost("{projectId}/single-user-group")]
        public IActionResult AddProjectToUserGroupByCode(long projectId, [FromBody] AssignPermissionToSingleUserGroupContract data)
        {
            m_authorizationManager.AuthorizeBookOrPermission(projectId, PermissionFlag.AdminProject, PermissionNames.AssignPermissionsToRoles);

            m_permissionManager.AddBookToSingleUserGroup(projectId, data.Code, data.Permissions);
            return Ok();
        }

        [HttpGet("{projectId}/group")]
        public ActionResult<ProjectGroupContract> GetProjectGroups(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            return m_projectGroupManager.GetProjectGroups(projectId);
        }

        [HttpPut("{targetProjectId}/group")]
        public IActionResult AddProjectToGroup(long targetProjectId, [FromQuery] long selectedProjectId)
        {
            m_authorizationManager.AuthorizeBook(selectedProjectId, PermissionFlag.EditProject);
            m_authorizationManager.AuthorizeBook(targetProjectId, PermissionFlag.ReadProject);

            m_projectGroupManager.AddProjectToGroup(targetProjectId, selectedProjectId);
            return Ok();
        }

        [HttpDelete("{projectId}/group")]
        public IActionResult RemoveProjectFromGroup(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectGroupManager.RemoveProjectFromGroup(projectId);
            return Ok();
        }

        [HttpGet("{projectId}/current-user-permission")]
        public ActionResult<PermissionDataContract> GetCurrentUserProjectPermissions(long projectId)
        {
            var result = m_authorizationManager.GetCurrentUserProjectPermissions(projectId);
            return result;
        }
    }
}