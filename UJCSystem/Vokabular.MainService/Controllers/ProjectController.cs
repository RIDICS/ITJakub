using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Parameter;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.Utils.Documentation;
using Vokabular.RestClient.Headers;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : BaseController
    {
        private readonly ProjectManager m_projectManager;
        private readonly ProjectMetadataManager m_projectMetadataManager;
        private readonly ProjectInfoManager m_projectInfoManager;

        public ProjectController(ProjectManager projectManager, ProjectMetadataManager projectMetadataManager,
            ProjectInfoManager projectInfoManager)
        {
            m_projectManager = projectManager;
            m_projectMetadataManager = projectMetadataManager;
            m_projectInfoManager = projectInfoManager;
        }
        
        [HttpGet]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, "int", "Total records count")]
        public List<ProjectDetailContract> GetProjectList([FromQuery] int? start, [FromQuery] int? count, [FromQuery] bool? fetchPageCount)
        {
            var result = m_projectManager.GetProjectList(start, count, fetchPageCount ?? false);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpGet("{projectId}")]
        [ProducesResponseType(typeof(ProjectDetailContract), StatusCodes.Status200OK)]
        public IActionResult GetProject(long projectId, [FromQuery] bool? fetchPageCount)
        {
            var projectData = m_projectManager.GetProject(projectId, fetchPageCount ?? false);
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
            [FromQuery] bool includeKind, [FromQuery] bool includeGenre, [FromQuery] bool includeOriginal)
        {
            var parameters = new GetProjectMetadataParameter
            {
                IncludeKind = includeKind,
                IncludeGenre = includeGenre,
                IncludeOriginal = includeOriginal,
                IncludeResponsiblePerson = includeResponsiblePerson,
                IncludeAuthor = includeAuthor
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
    }
}