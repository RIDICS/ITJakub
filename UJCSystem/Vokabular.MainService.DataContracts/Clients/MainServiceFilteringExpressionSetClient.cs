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
                        $"bibliography/filtering-expression-set?start={start}&count={count}&fetchPageCount={fetchPageCount}");
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
                var result = m_client.Get<IList<FilteringExpressionSetContract>>($"bibliography/filtering-expression-set/all");
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
                var result = m_client.Get<FilteringExpressionSetDetailContract>($"bibliography/filtering-expression-set/{filteringExpressionSetId}");
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
                var filteringExpressionSetId = m_client.Post<int>("bibliography/filtering-expression-set", filteringExpressionSet);
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
                m_client.Delete($"bibliography/filtering-expression-set/{filteringExpressionSetId}");
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
                m_client.Put<object>($"bibliography/filtering-expression-set/{filteringExpressionSetId}", filteringExpressionSet);
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
                return m_client.Get<IList<BibliographicFormatContract>>($"bibliography/bibliography-format");
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
