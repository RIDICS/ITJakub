using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.RestClient;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.RequestContracts;
using Vokabular.Shared.DataContracts.Search.ResultContracts;
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
                var result = Post<ResultContract>($"text", textResource);
                return result.Id;
            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void CreateSnapshot(long snapshotId, long projectId, List<string> pageIds)
        {
            var snapshotResource = new SnapshotPageIdsResourceContract { PageIds = pageIds, SnapshotId = snapshotId, ProjectId = projectId};

            try
            {
                var result = Post<ResultContract>($"snapshot", snapshotResource);
                
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
                var result = Post<FulltextSearchResultContract>($"snapshot/search", searchRequestContract);
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
                var result = Post<FulltextSearchResultContract>($"snapshot/search-count", searchRequest);
                return result;

            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PageSearchResultData SearchPageByCriteria(long snapshotId, List<SearchCriteriaContract> criteria)
        {
            try
            {
                var result = Post<PageSearchResultData>($"text/search?snapshotId={snapshotId}", new SearchRequestContractBase{ConditionConjunction = criteria});
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
                var result = Post<TextResourceContract>($"text/search/{resourceId}?formatValue={format}", searchRequest);
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
                var result = Post<long>($"corpus/search-count", searchRequest);
                return result;

            }
            catch (HttpRequestException e)
            {
                if (Logger.IsErrorEnabled())
                    Logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public CorpusSearchResultDataList SearchCorpusByCriteria(int start, int count, int contextLength, List<SearchCriteriaContract> searchCriterias)
        {
            var searchRequest = new CorpusSearchRequestContract { Start = start, Count = count, ContextLength = contextLength, ConditionConjunction = searchCriterias };
            try
            {
                var result = Post<CorpusSearchResultDataList>($"corpus/search", searchRequest);
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
