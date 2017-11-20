using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

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

        [HttpPost("text/{textId}")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateNewTextResourceVersion([FromBody] TextContract request)
        {
            return StatusCode(StatusCodes.Status409Conflict); // Version conflict
        }

        [HttpGet("text/{textId}/comment")]
        public List<GetTextCommentContract> GetCommentsForText(long textId)
        {
            var result = m_projectContentManager.GetCommentsForText(textId);
            return result;
        }

        [HttpPost("text/{textId}/comment")]
        public long CreateComment(long textId, [FromBody] CreateTextCommentContract request)
        {
            var resultId = m_projectContentManager.CreateNewComment(textId, request);
            return resultId;
        }

        [HttpDelete("text/comment/{commentId}")]
        public void DeleteComment(long commentId)
        {
            m_projectContentManager.DeleteComment(commentId);
        }
    }
}
