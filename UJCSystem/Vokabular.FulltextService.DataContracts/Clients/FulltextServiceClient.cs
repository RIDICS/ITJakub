using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.RestClient;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.Extensions;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.DataContracts.Clients
{
    public class FulltextServiceClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<FulltextServiceClient>();
        private readonly FulltextServiceRestClient m_client;

        public FulltextServiceClient(FulltextServiceRestClient client)
        {
            m_client = client;
        }

        public TextResourceContract GetTextResource(string resourceId, TextFormatEnumContract formatValue)
        {
            try
            {
                var textResource = m_client.Get<TextResourceContract>($"text/{resourceId}?formatValue={formatValue}");
                return textResource;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }


        public string CreateTextResource(string text, int versionNumber)
        {
            var textResource = new TextResourceContract{ PageText = text, VersionNumber  = versionNumber};

            try
            {
                var result = m_client.Post<ResultContract>("text", textResource);
                return result.Id;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void CreateSnapshot(SnapshotPageIdsResourceContract snapshotResource)
        {
            try
            {
                m_client.Post<ResultContract>("snapshot", snapshotResource);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FulltextSearchResultContract SearchByCriteria(SearchRequestContract searchRequestContract)
        {
            try
            {
                var result = m_client.Post<FulltextSearchResultContract>("snapshot/search", searchRequestContract);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FulltextSearchResultContract SearchByCriteriaCount(List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new SearchRequestContractBase {ConditionConjunction = searchCriterias};
            try
            {
                var result = m_client.Post<FulltextSearchResultContract>("snapshot/search-count", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PageSearchResultContract SearchPageByCriteria(long snapshotId, List<SearchCriteriaContract> searchCriterias)
        {
            try
            {
                var result = m_client.Post<PageSearchResultContract>($"text/snapshot/{snapshotId}/search", new SearchRequestContractBase{ConditionConjunction = searchCriterias});
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchHitsResultCount(long snapshotId, List<SearchCriteriaContract> searchCriterias)
        {
            try
            {
                var result = m_client.Post<long>($"text/snapshot/{snapshotId}/search-count", new SearchRequestContractBase { ConditionConjunction = searchCriterias });
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public TextResourceContract GetTextResourceFromSearch(string resourceId, TextFormatEnumContract format, SearchPageRequestContract searchRequest)
        {
            try
            {
                var result = m_client.Post<TextResourceContract>($"text/{resourceId}/search?formatValue={format}", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchCorpusByCriteriaCount(List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new SearchRequestContractBase { ConditionConjunction = searchCriterias };
            try
            {
                var result = m_client.Post<long>("corpus/search-count", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CorpusSearchResultContract> SearchCorpusByCriteria(int start, int count, int contextLength, List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new CorpusSearchRequestContract { Start = start, Count = count, ContextLength = contextLength, ConditionConjunction = searchCriterias };
            try
            {
                var result = m_client.Post<List<CorpusSearchResultContract>>("corpus/search", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public CorpusSearchSnapshotsResultContract SearchCorpusGetSnapshotListByCriteria(int start, int count, SortTypeEnumContract? sort, SortDirectionEnumContract? sortDirection, List<SearchCriteriaContract> searchCriterias, bool fetchNumberOfResults)
        {
            var searchRequest = new CorpusSearchRequestContract { Start = start, Count = count, ConditionConjunction = searchCriterias, FetchNumberOfResults = fetchNumberOfResults, Sort = sort, SortDirection = sortDirection };
            try
            {
                var result = m_client.Post<CorpusSearchSnapshotsResultContract>("bookpagedcorpus/search", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CorpusSearchResultContract> SearchCorpusInSnapshotByCriteria(long snapshotId, int start, int count, int contextLength, List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new CorpusSearchRequestContract { Start = start, Count = count, ContextLength = contextLength, ConditionConjunction = searchCriterias };
            try
            {
                var result = m_client.Post<List<CorpusSearchResultContract>>($"bookpagedcorpus/snapshot/{snapshotId}/search", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchCorpusTotalResultCount(List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new SearchRequestContractBase { ConditionConjunction = searchCriterias };
            try
            {
                var result = m_client.Post<long>("bookpagedcorpus/search-count", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public HitsWithPageContextResultContract SearchHitsWithPageContext(long snapshotId, int start, int count, int contextLength,
            List<SearchCriteriaContract> criteria)
        {
            var searchRequest = new SearchHitsRequestContract
            {
                Start = start,
                Count = count,
                ContextLength = contextLength,
                ConditionConjunction = criteria
            };

            try
            {
                var result = m_client.Post<HitsWithPageContextResultContract>($"text/snapshot/{snapshotId}/search-context", searchRequest);
                return result;
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
