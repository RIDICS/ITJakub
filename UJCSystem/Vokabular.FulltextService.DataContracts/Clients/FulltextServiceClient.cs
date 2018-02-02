using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.RestClient;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.Extensions;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.DataContracts.Clients
{
    public class FulltextServiceClient : FullRestClientBase
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<FulltextServiceClient>();

        public FulltextServiceClient(Uri baseAddress) : base(baseAddress)
        {
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {
        }

        public TextResourceContract GetTextResource(string resourceId, TextFormatEnumContract formatValue)
        {
            try
            {
                var textResource = Get<TextResourceContract>($"text/{resourceId}?formatValue={formatValue}");
                return textResource;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }


        public string CreateTextResource(string text, int versionNumber)
        {
            var textResource = new TextResourceContract{ PageText = text, VersionNumber  = versionNumber};

            try
            {
                var result = Post<ResultContract>("text", textResource);
                return result.Id;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void CreateSnapshot(SnapshotPageIdsResourceContract snapshotResource)
        {
            try
            {
                var result = Post<ResultContract>("snapshot", snapshotResource);
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public FulltextSearchResultContract SearchByCriteria(SearchRequestContract searchRequestContract)
        {
            try
            {
                var result = Post<FulltextSearchResultContract>("snapshot/search", searchRequestContract);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public FulltextSearchResultContract SearchByCriteriaCount(List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new SearchRequestContractBase {ConditionConjunction = searchCriterias};
            try
            {
                var result = Post<FulltextSearchResultContract>("snapshot/search-count", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PageSearchResultContract SearchPageByCriteria(long snapshotId, List<SearchCriteriaContract> criteria)
        {
            try
            {
                var result = Post<PageSearchResultContract>($"text/search?snapshotId={snapshotId}", new SearchRequestContractBase{ConditionConjunction = criteria});
                return result;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public TextResourceContract GetTextResourceFromSearch(string resourceId, TextFormatEnumContract format, SearchPageRequestContract searchRequest)
        {
            try
            {
                var result = Post<TextResourceContract>($"text/{resourceId}/search?formatValue={format}", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchCorpusByCriteriaCount(List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new SearchRequestContractBase { ConditionConjunction = searchCriterias };
            try
            {
                var result = Post<long>("corpus/search-count", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CorpusSearchResultContract> SearchCorpusByCriteria(int start, int count, int contextLength, List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new CorpusSearchRequestContract { Start = start, Count = count, ContextLength = contextLength, ConditionConjunction = searchCriterias };
            try
            {
                var result = Post<List<CorpusSearchResultContract>>("corpus/search", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public CorpusSearchSnapshotsResultContract SearchCorpusGetSnapshotListByCriteria(int start, int count, List<SearchCriteriaContract> searchCriterias, bool fetchNumberOfResults)
        {
            var searchRequest = new CorpusSearchRequestContract { Start = start, Count = count, ConditionConjunction = searchCriterias, FetchNumberOfResults = fetchNumberOfResults };
            try
            {
                var result = Post<CorpusSearchSnapshotsResultContract>("bookpagedcorpus/search", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CorpusSearchResultContract> SearchCorpusInSnapshotByCriteria(long snapshotId, int start, int count, int contextLength, List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new CorpusSearchRequestContract { Start = start, Count = count, ContextLength = contextLength, ConditionConjunction = searchCriterias };
            try
            {
                var result = Post<List<CorpusSearchResultContract>>($"bookpagedcorpus/snapshot/{snapshotId}/search", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchCorpusTotalResultCount(List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new SearchRequestContractBase { ConditionConjunction = searchCriterias };
            try
            {
                var result = Post<long>("bookpagedcorpus/search-count", searchRequest);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
    }
}
