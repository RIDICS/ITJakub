﻿using System;
using System.Collections.Generic;
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
        
        [HttpGet("project/{projectId}/resource")]
        public IList<ResourceWithLatestVersionContract> GetResourceList(long projectId, [FromQuery] ResourceTypeEnumContract? resourceType)
        {
            return m_projectContentManager.GetResourceList(projectId, resourceType);
        }

        [HttpDelete("resource/{resourceId}")]
        public void DeleteResource(long resourceId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("resource/{resourceId}/version")]
        public IList<ResourceVersionContract> GetResourceVersionHistory(long resourceId)
        {
            return m_resourceManager.GetResourceVersionHistory(resourceId);
        }
    }
}