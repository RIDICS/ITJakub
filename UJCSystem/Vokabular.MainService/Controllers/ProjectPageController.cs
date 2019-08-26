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
            var result = m_projectItemManager.GetPage(pageId);
            return result;
        }

        [HttpPost("{projectId}/page")]
        public IActionResult CreatePageResource(long projectId, [FromBody] CreatePageContract pageData)
        {
            var resourcePageId = m_projectItemManager.CreatePage(projectId, pageData);
            return Ok(resourcePageId);
        }

        [HttpPut("page/{pageId}")]
        public IActionResult UpdatePageResource(long pageId, [FromBody] CreatePageContract pageData)
        {
            m_projectItemManager.UpdatePage(pageId, pageData);
            return Ok();
        }

        [HttpPost("{projectId}/page-list")]
        public IActionResult SavePageList(long projectId, [FromBody] List<CreateOrUpdatePageContract> pageData)
        {
            m_projectItemManager.SavePages(projectId, pageData);
            return Ok();
        }

        [HttpGet("page/{pageId}/term")]
        public List<TermContract> GetPageTermList(long pageId)
        {
            var result = m_projectItemManager.GetPageTermList(pageId);
            return result;
        }

        [HttpPut("page/{pageId}/term")]
        public IActionResult SetTerms(long pageId, [FromBody] IntegerIdListContract termIdList)
        {
            m_projectItemManager.SetPageTerms(pageId, termIdList.IdList);
            return Ok();
        }
    }
}
