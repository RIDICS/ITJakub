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

        [HttpPost("image/{imageId}")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateNewImageResourceVersion(long imageId, [FromQuery] string fileName, [FromQuery] long? originalVersionId, [FromQuery] long? pageId, [FromQuery] string comment)
        {
            if (fileName == null || originalVersionId == null || pageId == null)
            {
                return BadRequest("Missing required parameters");
            }

            var stream = Request.Body;
            var contract = new CreateImageContract
            {
                Comment = comment,
                FileName = fileName,
                OriginalVersionId = originalVersionId.Value,
                ResourcePageId = pageId.Value,
            };

            var resultVersionId = m_projectContentManager.CreateNewImageVersion(imageId, contract, stream);
            return Ok(resultVersionId);
        }
    }
}