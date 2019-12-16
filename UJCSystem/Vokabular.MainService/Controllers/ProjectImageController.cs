using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectImageController : BaseController
    {
        private readonly ProjectContentManager m_projectContentManager;
        private readonly AuthorizationManager m_authorizationManager;

        public ProjectImageController(ProjectContentManager projectContentManager, AuthorizationManager authorizationManager)
        {
            m_projectContentManager = projectContentManager;
            m_authorizationManager = authorizationManager;
        }

        [HttpGet("{projectId}/image")]
        public List<ImageWithPageContract> GetImageList(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            var result = m_projectContentManager.GetImageResourceList(projectId);
            return result;
        }

        [HttpGet("page/{pageId}/image")]
        public ImageContract GetImageResourceByPageId(long pageId)
        {
            m_authorizationManager.AuthorizeResource(pageId, PermissionFlag.ReadProject);

            var result = m_projectContentManager.GetImageResourceByPageId(pageId);
            return result;
        }

        [HttpGet("image/{imageId}/content")]
        public IActionResult GetImage(long imageId)
        {
            m_authorizationManager.AuthorizeResource(imageId, PermissionFlag.ReadProject);

            var result = m_projectContentManager.GetImageResource(imageId);
            if (result == null)
                return NotFound();

            return File(result.Stream, result.MimeType, result.FileName, result.FileSize);
        }

        [HttpGet("image/version/{imageVersionId}/content")]
        public IActionResult GetImageVersion(long imageVersionId)
        {
            m_authorizationManager.AuthorizeResourceVersion(imageVersionId, PermissionFlag.ReadProject);

            var result = m_projectContentManager.GetImageResourceVersion(imageVersionId);
            if (result == null)
                return NotFound();

            return File(result.Stream, result.MimeType, result.FileName, result.FileSize);
        }

        [HttpPost("image")]
        [ProducesResponseType(typeof(NewResourceResultContract), StatusCodes.Status200OK)]
        public IActionResult CreateNewImageResourceVersion([FromForm] CreateImageContract data, IFormFile file)
        {
            // Authorization is below

            if ((data.ImageId == null || data.OriginalVersionId == null) && data.ResourcePageId == null)
            {
                return BadRequest("Image must be specified by ImageId + OriginalVersionId or by ResourcePageId");
            }

            if (data.ResourcePageId != null)
            {
                m_authorizationManager.AuthorizeResource(data.ResourcePageId.Value, PermissionFlag.EditProject);
            }
            else
            {
                m_authorizationManager.AuthorizeResource(data.ImageId.Value, PermissionFlag.EditProject);
            }

            var result = m_projectContentManager.CreateNewImageVersion(data, file.OpenReadStream());
            return Ok(result);
        }
    }
}