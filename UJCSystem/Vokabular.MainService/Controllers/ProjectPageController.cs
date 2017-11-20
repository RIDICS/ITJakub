using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectPageController : BaseController
    {
        private readonly ProjectItemManager m_projectItemManager;

        public ProjectPageController(ProjectItemManager projectItemManager)
        {
            m_projectItemManager = projectItemManager;
        }

        [HttpGet("{projectId}/page")]
        public List<PageContract> GetPageList(long projectId)
        {
            var result = m_projectItemManager.GetPageList(projectId);
            return result;
        }

        [HttpGet("page/{pageId}")]
        public PageContract GetPageResource(long pageId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{projectId}/page")]
        public IActionResult CreatePageResource(long projectId, [FromBody] CreatePageContract pageData)
        {
            throw new NotImplementedException();
        }

        [HttpPut("page/{pageId}")]
        public IActionResult UpdatePageResource(long pageId, [FromBody] CreatePageContract pageData)
        {
            throw new NotImplementedException();
        }

        [HttpGet("page/{pageId}/term")]
        public List<TermContract> GetPageTermList(long pageId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("page/{pageId}/term")]
        public IActionResult SetTerms(long pageId, IntegerIdListContract termIdList)
        {
            throw new NotImplementedException();
        }
    }
}
