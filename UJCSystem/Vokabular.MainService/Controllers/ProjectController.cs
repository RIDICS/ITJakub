using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Parameter;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Headers;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private const int DefaultStartItem = 0;
        private const int DefaultProjectItemCount = 5;

        private readonly ProjectManager m_projectManager;
        private readonly ProjectMetadataManager m_projectMetadataManager;

        public ProjectController(ProjectManager projectManager, ProjectMetadataManager projectMetadataManager)
        {
            m_projectManager = projectManager;
            m_projectMetadataManager = projectMetadataManager;
        }

        [HttpGet]
        public List<ProjectContract> GetProjectList([FromQuery] int? start, [FromQuery] int? count)
        {
            if (start == null)
            {
                start = DefaultStartItem;
            }
            if (count == null)
            {
                count = DefaultProjectItemCount;
            }

            var result = m_projectManager.GetProjectList(start.Value, count.Value);

            Response.Headers.Add(CustomHttpHeaders.TotalCount, result.TotalCount.ToString());
            return result.List;
        }

        [HttpGet("{projectId}")]
        [ProducesResponseType(typeof(ProjectContract), StatusCodes.Status200OK)]
        public IActionResult GetProject(long projectId)
        {
            var projectData = m_projectManager.GetProject(projectId);
            if (projectData == null)
                return NotFound();

            return Ok(projectData);
        }

        [HttpPost]
        public long CreateProject([FromBody] ProjectContract project)
        {
            return m_projectManager.CreateProject(project);
        }

        [HttpDelete("{projectId}")]
        public void DeleteProject(long projectId)
        {
            throw new System.NotImplementedException();
        }

        [HttpGet("{projectId}/metadata")]
        [ProducesResponseType(typeof(ProjectMetadataResultContract), StatusCodes.Status200OK)]
        public IActionResult GetProjectMetadata(long projectId, [FromQuery] bool includeAuthor, [FromQuery] bool includeResponsiblePerson,
            [FromQuery] bool includeKind, [FromQuery] bool includeGenre)
        {
            var parameters = new GetProjectMetadataParameter
            {
                IncludeKind = includeKind,
                IncludeGenre = includeGenre,
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

        [HttpPut("{projectId}/literarykind")]
        public void SetLiteraryKinds(long projectId, [FromBody] IntegerIdListContract kindIdList)
        {
            m_projectMetadataManager.SetLiteraryKinds(projectId, kindIdList);
        }

        [HttpPut("{projectId}/literarygenre")]
        public void SetLiteraryGenres(long projectId, [FromBody] IntegerIdListContract genreIdList)
        {
            m_projectMetadataManager.SetLiteraryGenres(projectId, genreIdList);
        }

        [HttpPut("{projectId}/author")]
        public void SetAuthors(long projectId, [FromBody] IntegerIdListContract authorIdList)
        {
            m_projectMetadataManager.SetAuthors(projectId, authorIdList);
        }

        [HttpPut("{projectId}/responsibleperson")]
        public void SetResponsiblePersons(long projectId, [FromBody] IntegerIdListContract responsiblePersonIdList)
        {
            m_projectMetadataManager.SetResponsiblePersons(projectId, responsiblePersonIdList);
        }
    }
}