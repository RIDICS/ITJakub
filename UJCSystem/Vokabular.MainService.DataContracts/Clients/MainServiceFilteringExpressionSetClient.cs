using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceFilteringExpressionSetClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceFilteringExpressionSetClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public PagedResultList<FilteringExpressionSetDetailContract> GetFilteringExpressionSetList(int start, int count, bool fetchPageCount = false)
        {
            try
            {
                var result =
                    m_client.GetPagedList<FilteringExpressionSetDetailContract>(
                        $"filteringExpressionSet?start={start}&count={count}&fetchPageCount={fetchPageCount}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<FilteringExpressionSetContract> GetAllFilteringExpressionSets()
        {
            try
            {
                var result = m_client.Get<IList<FilteringExpressionSetContract>>($"filteringExpressionSet/allFilteringExpressionSets");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FilteringExpressionSetDetailContract GetFilteringExpressionSetDetail(int filteringExpressionSetId)
        {
            try
            {
                var result = m_client.Get<FilteringExpressionSetDetailContract>($"filteringExpressionSet/{filteringExpressionSetId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateFilteringExpressionSet(FilteringExpressionSetDetailContract filteringExpressionSet)
        {
            try
            {
                var filteringExpressionSetId = m_client.Post<int>("filteringExpressionSet", filteringExpressionSet);
                return filteringExpressionSetId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteFilteringExpressionSet(int filteringExpressionSetId)
        {
            try
            {
                m_client.Delete($"filteringExpressionSet/{filteringExpressionSetId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateFilteringExpressionSet(int filteringExpressionSetId, FilteringExpressionSetDetailContract filteringExpressionSet)
        {
            try
            {
                m_client.Put<object>($"filteringExpressionSet/{filteringExpressionSetId}", filteringExpressionSet);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<BibliographicFormatContract> GetAllBibliographicFormats()
        {
            try
            {
                return m_client.Get<IList<BibliographicFormatContract>>($"filteringExpressionSet/allBibliographicFormats");
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
