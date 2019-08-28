using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceFeedbackClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceFeedbackClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public long CreateFeedback(CreateFeedbackContract data)
        {
            try
            {
                var result = m_client.Post<long>("feedback", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateAnonymousFeedback(CreateAnonymousFeedbackContract data)
        {
            try
            {
                var result = m_client.Post<long>("feedback/anonymous", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateHeadwordFeedback(long resourceVersionId, CreateFeedbackContract data)
        {
            try
            {
                var result = m_client.Post<long>($"feedback/headword/version/{resourceVersionId}", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateAnonymousHeadwordFeedback(long resourceVersionId, CreateAnonymousFeedbackContract data)
        {
            try
            {
                var result = m_client.Post<long>($"feedback/headword/version/{resourceVersionId}/anonymous", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<FeedbackContract> GetFeedbackList(int start,
            int count,
            FeedbackSortEnumContract sort,
            SortDirectionEnumContract sortDirection,
            IList<FeedbackCategoryEnumContract> filterCategories)
        {
            try
            {
                var result = m_client.GetPagedList<FeedbackContract>("feedback");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteFeedback(long feedbackId)
        {
            try
            {
                m_client.Delete($"feedback/{feedbackId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
    }
}
