﻿using System;
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
        private readonly ResourceManager m_resourceManager;


        public ResourceController(ProjectContentManager projectContentManager, ResourceManager resourceManager)
        {
            m_projectContentManager = projectContentManager;
            m_resourceManager = resourceManager;
        }
        // TODO determine which methods are required and finish their implementation (or do any required modification). Remove other non-required methods.
        
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
            return m_resourceManager.GetResourceVersionHistory(resourceId);
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