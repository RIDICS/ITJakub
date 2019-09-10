using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectTextController : BaseController
    {
        private readonly ProjectContentManager m_projectContentManager;

        public ProjectTextController(ProjectContentManager projectContentManager)
        {
            m_projectContentManager = projectContentManager;
        }

        [HttpGet("{projectId}/text")]
        public List<TextWithPageContract> GetTextResourceList(long projectId, [FromQuery] long? resourceGroupId)
        {
            var result = m_projectContentManager.GetTextResourceList(projectId, resourceGroupId);
            return result;
        }

        [HttpGet("text/{textId}")]
        public FullTextContract GetTextResource(long textId, [FromQuery] TextFormatEnumContract? format)
        {
            if (format == null)
                format = TextFormatEnumContract.Html;

            var result = m_projectContentManager.GetTextResource(textId, format.Value);
            return result;
        }

        [HttpGet("text/version/{textVersionId}")]
        public FullTextContract GetTextResourceVersion(long textVersionId, [FromQuery] TextFormatEnumContract? format)
        {
            if (format == null)
                format = TextFormatEnumContract.Html;

            var result = m_projectContentManager.GetTextResourceVersion(textVersionId, format.Value);
            return result;
        }

        [HttpPost("text/{textId}")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateNewTextResourceVersion([FromBody] CreateTextRequestContract request)
        {
            var result = m_projectContentManager.CreateNewTextResourceVersion(request);
            return Ok(result);
            //TODO check result
            //return StatusCode(StatusCodes.Status409Conflict); // Version conflict
        }

        [HttpGet("text/{textId}/comment")]
        public List<GetTextCommentContract> GetCommentsForText(long textId)
        {
            var result = m_projectContentManager.GetCommentsForText(textId);
            return result;
        }

        [HttpPost("text/{textId}/comment")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateComment(long textId, [FromBody] CreateTextCommentContract request)
        {
            try
            {
                var resultId = m_projectContentManager.CreateNewComment(textId, request);
                return Ok(resultId);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpPut("text/comment/{commentId}")]
        public IActionResult UpdateComment(long commentId, [FromBody] UpdateTextCommentContract request)
        {
            try
            {
                m_projectContentManager.UpdateComment(commentId, request);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        [HttpDelete("text/comment/{commentId}")]
        public void DeleteComment(long commentId)
        {
            m_projectContentManager.DeleteComment(commentId);
        }
    }
}
