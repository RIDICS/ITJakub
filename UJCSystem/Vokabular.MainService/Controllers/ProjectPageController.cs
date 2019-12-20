using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectPageController : BaseController
    {
        private readonly ProjectItemManager m_projectItemManager;
        private readonly AuthorizationManager m_authorizationManager;

        public ProjectPageController(ProjectItemManager projectItemManager, AuthorizationManager authorizationManager)
        {
            m_projectItemManager = projectItemManager;
            m_authorizationManager = authorizationManager;
        }

        [HttpGet("{projectId}/page")]
        public List<PageContract> GetPageList(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            var result = m_projectItemManager.GetPageList(projectId);
            return result;
        }
        
        [HttpGet("{projectId}/page-image-info")]
        public List<PageWithImageInfoContract> GetAllPagesWithImageInfoList(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            var result = m_projectItemManager.GetPageWithImageInfoList(projectId);
            return result;
        }

        [HttpGet("page/{pageId}")]
        public PageContract GetPageResource(long pageId)
        {
            m_authorizationManager.AuthorizeResource(pageId, PermissionFlag.ReadProject);

            var result = m_projectItemManager.GetPage(pageId);
            return result;
        }

        [HttpPost("{projectId}/page")]
        public IActionResult CreatePageResource(long projectId, [FromBody] CreatePageContract pageData)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            var resourcePageId = m_projectItemManager.CreatePage(projectId, pageData);
            return Ok(resourcePageId);
        }

        [HttpPut("page/{pageId}")]
        public IActionResult UpdatePageResource(long pageId, [FromBody] CreatePageContract pageData)
        {
            m_authorizationManager.AuthorizeResource(pageId, PermissionFlag.EditProject);

            m_projectItemManager.UpdatePage(pageId, pageData);
            return Ok();
        }

        [HttpPut("{projectId}/page")]
        public IActionResult UpdatePageList(long projectId, [FromBody] List<CreateOrUpdatePageContract> pageData)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectItemManager.UpdatePages(projectId, pageData);
            return Ok();
        }

        [HttpGet("page/{pageId}/term")]
        public List<TermContract> GetPageTermList(long pageId)
        {
            m_authorizationManager.AuthorizeResource(pageId, PermissionFlag.ReadProject);

            var result = m_projectItemManager.GetPageTermList(pageId);
            return result;
        }

        [HttpPut("page/{pageId}/term")]
        public IActionResult SetTerms(long pageId, [FromBody] IntegerIdListContract termIdList)
        {
            m_authorizationManager.AuthorizeResource(pageId, PermissionFlag.EditProject);

            m_projectItemManager.SetPageTerms(pageId, termIdList.IdList);
            return Ok();
        }

        [HttpGet("page/{pageId}/text-content")]
        public IActionResult GetPageText(long pageId, [FromQuery] TextFormatEnumContract? format)
        {
            m_authorizationManager.AuthorizeResource(pageId, PermissionFlag.ReadProject);

            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_projectItemManager.GetPageText(pageId, formatValue);
            if (result == null)
                return NotFound();

            return Content(result);
        }

        [HttpGet("page/{pageId}/image-content")]
        public IActionResult GetPageImage(long pageId)
        {
            m_authorizationManager.AuthorizeResource(pageId, PermissionFlag.ReadProject);

            var result = m_projectItemManager.GetPageImage(pageId);
            if (result == null)
                return NotFound();

            Response.ContentLength = result.FileSize;
            return File(result.Stream, result.MimeType, result.FileName);
        }

        [HttpHead("page/{pageId}/image")]
        public IActionResult HasPageImage(long pageId)
        {
            m_authorizationManager.AuthorizeResource(pageId, PermissionFlag.ReadProject);

            var hasImage = m_projectItemManager.HasBookPageImage(pageId);
            return hasImage ? (IActionResult)Ok() : NotFound();
        }
    }
}
