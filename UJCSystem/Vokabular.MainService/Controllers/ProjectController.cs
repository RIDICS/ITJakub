using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Parameter;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
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

        public ProjectController(ProjectManager projectManager, ProjectMetadataManager projectMetadataManager,
            ProjectInfoManager projectInfoManager, ForumSiteManager forumSiteManager)
        {
            m_projectManager = projectManager;
            m_projectMetadataManager = projectMetadataManager;
            m_projectInfoManager = projectInfoManager;
            m_forumSiteManager = forumSiteManager;
        }
        
        [HttpGet]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total records count")]
        public List<ProjectDetailContract> GetProjectList([FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByName, [FromQuery] bool? fetchPageCount, [FromQuery] bool? fetchAuthors, [FromQuery] bool? fetchResponsiblePersons)
        {
            var isFetchPageCount = fetchPageCount ?? false;
            var isFetchAuthors = fetchAuthors ?? false;
            var isFetchResponsiblePersons = fetchResponsiblePersons ?? false;
            var result = m_projectManager.GetProjectList(start, count, filterByName, isFetchPageCount, isFetchAuthors, isFetchResponsiblePersons);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpGet("{projectId}")]
        [ProducesResponseType(typeof(ProjectDetailContract), StatusCodes.Status200OK)]
        public IActionResult GetProject(long projectId, [FromQuery] bool? fetchPageCount, [FromQuery] bool? fetchAuthors, [FromQuery] bool? fetchResponsiblePersons)
        {
            var isFetchPageCount = fetchPageCount ?? false;
            var isFetchAuthors = fetchAuthors ?? false;
            var isFetchResponsiblePersons = fetchResponsiblePersons ?? false;

            var projectData = m_projectManager.GetProject(projectId, isFetchPageCount, isFetchAuthors, isFetchResponsiblePersons);
            if (projectData == null)
                return NotFound();

            return Ok(projectData);
        }

        [HttpPost]
        public long CreateProject([FromBody] ProjectContract project)
        {
            return m_projectManager.CreateProject(project);
        }

        [HttpPut("{projectId}")]
        public void UpdateProject(long projectId, [FromBody] ProjectContract data)
        {
            m_projectManager.UpdateProject(projectId, data);
        }

        [HttpDelete("{projectId}")]
        public void DeleteProject(long projectId)
        {
            m_projectManager.DeleteProject(projectId);
        }

        [HttpGet("{projectId}/metadata")]
        [ProducesResponseType(typeof(ProjectMetadataResultContract), StatusCodes.Status200OK)]
        public IActionResult GetProjectMetadata(long projectId, [FromQuery] bool includeAuthor, [FromQuery] bool includeResponsiblePerson,
            [FromQuery] bool includeKind, [FromQuery] bool includeGenre, [FromQuery] bool includeOriginal, [FromQuery] bool includeKeyword, [FromQuery] bool includeCategory)
        {
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
            return m_projectMetadataManager.CreateNewProjectMetadataVersion(projectId, metadata);
        }

        [HttpPut("{projectId}/literary-kind")]
        public void SetLiteraryKinds(long projectId, [FromBody] IntegerIdListContract kindIdList)
        {
            m_projectInfoManager.SetLiteraryKinds(projectId, kindIdList);
        }

        [HttpPut("{projectId}/literary-genre")]
        public void SetLiteraryGenres(long projectId, [FromBody] IntegerIdListContract genreIdList)
        {
            m_projectInfoManager.SetLiteraryGenres(projectId, genreIdList);
        }

        [HttpPut("{projectId}/literary-original")]
        public void SetLiteraryOriginal(long projectId, [FromBody] IntegerIdListContract litOriginalIdList)
        {
            m_projectInfoManager.SetLiteraryOriginals(projectId, litOriginalIdList);
        }

        [HttpPut("{projectId}/keyword")]
        public void SetKeywords(long projectId, [FromBody] IntegerIdListContract keywordIdList)
        {
            m_projectInfoManager.SetKeywords(projectId, keywordIdList);
        }

        [HttpPut("{projectId}/category")]
        public void SetCategories(long projectId, [FromBody] IntegerIdListContract categoryIdList)
        {
            m_projectInfoManager.SetCategories(projectId, categoryIdList);
        }

        [HttpPut("{projectId}/author")]
        public void SetAuthors(long projectId, [FromBody] IntegerIdListContract authorIdList)
        {
            m_projectInfoManager.SetAuthors(projectId, authorIdList);
        }

        [HttpPut("{projectId}/responsible-person")]
        public void SetResponsiblePersons(long projectId, [FromBody] List<ProjectResponsiblePersonIdContract> projectResposibleIdList)
        {
            m_projectInfoManager.SetResponsiblePersons(projectId, projectResposibleIdList);
        }

        [HttpGet("{projectId}/literary-kind")]
        public List<LiteraryKindContract> GetLiteraryKinds(long projectId)
        {
            return m_projectInfoManager.GetLiteraryKinds(projectId);
        }

        [HttpGet("{projectId}/literary-genre")]
        public List<LiteraryGenreContract> GetLiteraryGenres(long projectId)
        {
            return m_projectInfoManager.GetLiteraryGenres(projectId);
        }

        [HttpGet("{projectId}/literary-original")]
        public List<LiteraryOriginalContract> GetLiteraryOriginal(long projectId)
        {
            return m_projectInfoManager.GetLiteraryOriginals(projectId);
        }

        [HttpGet("{projectId}/keyword")]
        public List<KeywordContract> GetKeywords(long projectId)
        {
            return m_projectInfoManager.GetKeywords(projectId);
        }

        [HttpGet("{projectId}/category")]
        public List<CategoryContract> GetCategories(long projectId)
        {
            return m_projectInfoManager.GetCategories(projectId);
        }

        [HttpGet("{projectId}/author")]
        public List<OriginalAuthorContract> GetAuthors(long projectId)
        {
            return m_projectInfoManager.GetAuthors(projectId);
        }

        [HttpGet("{projectId}/responsible-person")]
        public List<ProjectResponsiblePersonContract> GetProjectResponsiblePersons(long projectId)
        {
            return m_projectInfoManager.GetProjectResponsiblePersons(projectId);
        }

        [HttpGet("{projectId}/role")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total records count")]
        public List<RoleContract> GetRolesByProject(long projectId, [FromQuery] int? start, [FromQuery] int? count,
            [FromQuery] string filterByName)
        {
            var result = m_projectManager.GetRolesByProject(projectId, start, count, filterByName);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpGet("{projectId}/forum")]
        [ProducesResponseType(typeof(ForumContract), StatusCodes.Status200OK)]
        public IActionResult GetForum(long projectId)
        {
            var forum = m_forumSiteManager.GetForum(projectId);
            return Ok(forum);
        }

        [HttpPost("{projectId}/forum")]
        public ActionResult<int> CreateForum(long projectId)
        {
            var forumId = m_forumSiteManager.CreateOrUpdateForums(projectId);

            return forumId != null ? (ActionResult<int>) Ok(forumId.Value) : BadRequest("Forum is disabled");
        }
    }
}