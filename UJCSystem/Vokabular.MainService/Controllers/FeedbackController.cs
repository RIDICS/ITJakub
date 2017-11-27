using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
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
        public long CreateFeedback([FromBody] CreateFeedbackContract data)
        {
            var resultId = m_feedbackManager.CreateFeedback(data);
            return resultId;
        }

        [HttpPost("anonymous")]
        public long CreateAnonymousFeedback([FromBody] CreateAnonymousFeedbackContract data)
        {
            var resultId = m_feedbackManager.CreateAnonymousFeedback(data);
            return resultId;
        }

        [HttpPost("resource/version/{resourceVersionId}")]
        public long CreateResourceFeedback(long resourceVersionId, [FromBody] CreateFeedbackContract data)
        {
            var resultId = m_feedbackManager.CreateResourceFeedback(resourceVersionId, data);
            return resultId;
        }

        [HttpPost("resource/version/{resourceVersionId}/anonymous")]
        public long CreateAnonymousResourceFeedback(long resourceVersionId, [FromBody] CreateFeedbackContract data)
        {
            var resultId = m_feedbackManager.CreateAnonymousResourceFeedback(resourceVersionId, data);
            return resultId;
        }

        [HttpGet("")]
        public PagedResultList<FeedbackContract> GetFeedbackList([FromQuery] int? start,
            [FromQuery] int? count,
            [FromQuery] FeedbackSortEnumContract? sort,
            [FromQuery] SortDirectionEnumContract? sortDirection,
            [FromQuery] IList<FeedbackCategoryEnumContract> filterCategories)
        {
            var sortValue = sort ?? FeedbackSortEnumContract.Date;
            var sortDirectionValue = sortDirection ?? SortDirectionEnumContract.Asc;

            var result = m_feedbackManager.GetFeedbackList(start, count, sortValue, sortDirectionValue, filterCategories);
            return result;
        }

        [HttpDelete("{feedbackId}")]
        public void DeleteFeedback(long feedbackId)
        {
            m_feedbackManager.DeleteFeedback(feedbackId);
        }
    }
}
