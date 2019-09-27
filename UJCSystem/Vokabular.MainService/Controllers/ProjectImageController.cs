using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectImageController : BaseController
    {
        private readonly ProjectContentManager m_projectContentManager;

        public ProjectImageController(ProjectContentManager projectContentManager)
        {
            m_projectContentManager = projectContentManager;
        }

        [HttpGet("{projectId}/image")]
        public List<ImageWithPageContract> GetImageList(long projectId)
        {
            var result = m_projectContentManager.GetImageResourceList(projectId);
            return result;
        }

        [HttpGet("page/{pageId}/image")]
        public ImageContract GetImageResourceByPageId(long pageId)
        {
            var result = m_projectContentManager.GetImageResourceByPageId(pageId);
            return result;
        }

        [HttpGet("image/{imageId}")]
        public IActionResult GetImage(long imageId)
        {
            var result = m_projectContentManager.GetImageResource(imageId);
            if (result == null)
                return NotFound();

            return File(result.Stream, result.MimeType, result.FileName, result.FileSize);
        }

        [HttpGet("image/version/{imageVersionId}")]
        public IActionResult GetImageVersion(long imageVersionId)
        {
            var result = m_projectContentManager.GetImageResourceVersion(imageVersionId);
            if (result == null)
                return NotFound();

            return File(result.Stream, result.MimeType, result.FileName, result.FileSize);
        }

        [HttpPost("image")]
        [ProducesResponseType(typeof(NewResourceResultContract), StatusCodes.Status200OK)]
        public IActionResult CreateNewImageResourceVersion([FromForm] CreateImageContract data, IFormFile file)
        {
            if ((data.ImageId == null || data.OriginalVersionId == null) && data.ResourcePageId == null)
            {
                return BadRequest("Image must be specified by ImageId + OriginalVersionId or by ResourcePageId");
            }

            var result = m_projectContentManager.CreateNewImageVersion(data, file.OpenReadStream());
            return Ok(result);
        }
    }
}