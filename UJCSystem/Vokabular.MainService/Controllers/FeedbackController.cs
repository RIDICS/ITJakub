using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class FeedbackController : BaseController
    {
        private readonly FeedbackManager m_feedbackManager;

        public FeedbackController(FeedbackManager feedbackManager)
        {
            m_feedbackManager = feedbackManager;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateFeedback([FromBody] CreateFeedbackContract data)
        {
            try
            {
                var resultId = m_feedbackManager.CreateFeedback(data);
                return Ok(resultId);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        [HttpPost("anonymous")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateAnonymousFeedback([FromBody] CreateAnonymousFeedbackContract data)
        {
            try
            {
                var resultId = m_feedbackManager.CreateAnonymousFeedback(data);
                return Ok(resultId);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpPost("headword/version/{resourceVersionId}")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateHeadwordFeedback(long resourceVersionId, [FromBody] CreateFeedbackContract data)
        {
            try
            {
                var resultId = m_feedbackManager.CreateHeadwordFeedback(resourceVersionId, data);
                return Ok(resultId);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpPost("headword/version/{resourceVersionId}/anonymous")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateAnonymousHeadwordFeedback(long resourceVersionId, [FromBody] CreateAnonymousFeedbackContract data)
        {
            try
            {
                var resultId = m_feedbackManager.CreateAnonymousHeadwordFeedback(resourceVersionId, data);
                return Ok(resultId);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [Authorize(PermissionNames.ManageFeedbacks)]
        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<FeedbackContract> GetFeedbackList([FromQuery] int? start,
            [FromQuery] int? count,
            [FromQuery] FeedbackSortEnumContract? sort,
            [FromQuery] SortDirectionEnumContract? sortDirection,
            [FromQuery] IList<FeedbackCategoryEnumContract> filterCategories)
        {
            var sortValue = sort ?? FeedbackSortEnumContract.Date;
            var sortDirectionValue = sortDirection ?? SortDirectionEnumContract.Desc;

            var result = m_feedbackManager.GetFeedbackList(start, count, sortValue, sortDirectionValue, filterCategories);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [Authorize(PermissionNames.ManageFeedbacks)]
        [HttpDelete("{feedbackId}")]
        public void DeleteFeedback(long feedbackId)
        {
            m_feedbackManager.DeleteFeedback(feedbackId);
        }
    }
}
