using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectResourceController : BaseController
    {
        private readonly ProjectContentManager m_projectContentManager;
        private readonly ResourceManager m_resourceManager;
        private readonly AuthorizationManager m_authorizationManager;

        public ProjectResourceController(ProjectContentManager projectContentManager, ResourceManager resourceManager, AuthorizationManager authorizationManager)
        {
            m_projectContentManager = projectContentManager;
            m_resourceManager = resourceManager;
            m_authorizationManager = authorizationManager;
        }
        
        [HttpGet("{projectId}/resource")]
        public IList<ResourceWithLatestVersionContract> GetResourceList(long projectId, [FromQuery] ResourceTypeEnumContract? resourceType)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            return m_projectContentManager.GetResourceList(projectId, resourceType);
        }

        [HttpDelete("resource/{resourceId}")]
        public void RemoveResource(long resourceId)
        {
            m_authorizationManager.AuthorizeResource(resourceId, PermissionFlag.EditProject);

            m_projectContentManager.RemoveResource(resourceId);
        }

        [HttpGet("resource/{resourceId}/version")]
        public ActionResult<IList<ResourceVersionContract>> GetResourceVersionHistory(long resourceId, int? higherVersion, int? lowerVersion)
        {
            m_authorizationManager.AuthorizeResource(resourceId, PermissionFlag.ReadProject);

            if (lowerVersion == null)
            {
                return BadRequest($"{nameof(lowerVersion)} parameter is required");
            }
            
            var result = m_resourceManager.GetResourceVersionHistory(resourceId, higherVersion, lowerVersion.Value);
            return Ok(result);
        }
    }
}