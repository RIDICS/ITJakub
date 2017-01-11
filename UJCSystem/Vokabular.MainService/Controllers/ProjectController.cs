using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Headers;
using Vokabular.MainService.DataContracts.ServiceContracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : Controller, IProjectMainService
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
        public ProjectContract GetProject(long projectId)
        {
            return m_projectManager.GetProject(projectId);
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
        public ProjectMetadataResultContract GetProjectMetadata(long projectId)
        {
            return m_projectMetadataManager.GetProjectMetadata(projectId);
        }

        [HttpPost("{projectId}/metadata")]
        public long CreateNewProjectMetadataVersion(long projectId, [FromBody] ProjectMetadataContract metadata)
        {
            return m_projectMetadataManager.CreateNewProjectMetadataVersion();
        }
    }
}
