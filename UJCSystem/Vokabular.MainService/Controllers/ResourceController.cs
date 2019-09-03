using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Controllers
{
    [Route("api")]
    public class ResourceController : BaseController
    {
        private readonly ProjectContentManager m_projectContentManager;


        public ResourceController(ProjectContentManager projectContentManager)
        {
            m_projectContentManager = projectContentManager;
        }
        // TODO determine which methods are required and finish their implementation (or do any required modification). Remove other non-required methods.

        [HttpPost("project/{projectId}/resource")]
        public long ProcessUploadedResources(long projectId, [FromBody] NewResourceContract resourceInfo)
        {
            return 22;
        }

        [HttpPost("resource/{resourceId}/version")]
        public long ProcessUploadedResourceVersion(long resourceId, [FromBody] NewResourceContract resourceInfo)
        {
            return 231;
        }

        [HttpGet("project/{projectId}/resource")]
        public IList<ResourceWithLatestVersionContract> GetResourceList(long projectId, [FromQuery] ResourceTypeEnumContract? resourceType)
        {
            return m_projectContentManager.GetResourceList(projectId, resourceType);
        }

        [HttpDelete("resource/{resourceId}")]
        public void DeleteResource(long resourceId)
        {
            
        }

        [HttpPut("resource/{resourceId}")]
        public void RenameResource(long resourceId, [FromBody] ResourceContract resource)
        {
            
        }

        [HttpPost("resource/{resourceId}/duplicate")]
        public long DuplicateResource(long resourceId)
        {
            return 45;
        }

        [HttpGet("resource/{resourceId}/version")]
        public IList<ResourceVersionContract> GetResourceVersionHistory(long resourceId)
        {
            return m_projectContentManager.GetResourceVersionHistory(resourceId);
        }

        [HttpGet("resource/{resourceId}/metadata")]
        [ProducesResponseType(typeof(ResourceMetadataContract), StatusCodes.Status200OK)]
        public IActionResult GetResourceMetadata(long resourceId)
        {
            var resultData = new ResourceMetadataContract
            {
                Editor = "Jan Novák",
                Editor2 = "Josef Novák",
                LastModification = DateTime.Now,
                EditionNote = "xxxxxxx"
            };
            return Ok(resultData);
        }
    }
}