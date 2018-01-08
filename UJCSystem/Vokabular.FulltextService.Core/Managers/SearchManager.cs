using System;
using System.Linq;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.RequestContracts;
using Vokabular.Shared.DataContracts.Search.ResultContracts;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SearchManager : ElasticsearchManagerBase
    {
        private const int FragmentSize = 50;
        private const int FragmentsCount = 1000000;
        private const int DefaultStart = 0;
        private const int DefaultSize = 10000;
        private const string HighlightTag = "$";
        private const string EmphTag = "*";
        private const string HighlighterType = "experimental";

        private readonly QueriesBuilder m_queriesBuilder;
        private readonly SearchResultProcessor m_searchResultProcessor;

        public SearchManager(CommunicationProvider communicationProvider, SearchResultProcessor searchResultProcessor,
            QueriesBuilder queriesBuilder) : base(communicationProvider)
        {
            m_searchResultProcessor = searchResultProcessor;
            m_queriesBuilder = queriesBuilder;
        }

        public FulltextSearchResultContract SearchByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var filterQuery =
                m_queriesBuilder.GetFilterSearchQuery(searchRequest.ConditionConjunction, SnapshotIdField);
            var mustQuery = m_queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);

            var client = CommunicationProvider.GetElasticClient();

            var response = client.Count<SnapshotResourceContract>(s => s
                .Index(SnapshotIndex)
                .Type(SnapshotType)
                .Query(q => q
                    .Bool(b => b
                        .Filter(filterQuery)
                        .Must(mustQuery)
                    )
                )
            );

            return m_searchResultProcessor.ProcessSearchByCriteriaCount(response);
        }

        public FulltextSearchResultContract SearchByCriteria(SearchRequestContract searchRequest)
        {
            var filterQuery =
                m_queriesBuilder.GetFilterSearchQuery(searchRequest.ConditionConjunction, SnapshotIdField);
            var mustQuery = m_queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);


            var client = CommunicationProvider.GetElasticClient();

            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(SnapshotIndex)
                .Type(SnapshotType)
                .From(searchRequest.Start ?? DefaultStart)
                .Size(searchRequest.Count ?? DefaultSize)
                .Source(sf => sf
                    .Includes(i => i
                        .Fields(
                            f => f.ProjectId
                        )
                    )
                )
                .Query(q => q
                    .Bool(b => b
                        .Filter(filterQuery)
                        .Must(mustQuery)
                    )
                )
                .Sort(so =>
                {
                    if (searchRequest.SortDirection.HasValue && searchRequest.Sort.HasValue)
                    {
                        if (searchRequest.SortDirection.Value == SortDirectionEnumContract.Asc)
                        {
                            return so.Ascending(GetElasticFieldName(searchRequest.Sort.Value));
                        }
                    
                        return so.Descending(GetElasticFieldName(searchRequest.Sort.Value));
                    }

                    return so.Ascending(GetElasticFieldName(SortTypeEnumContract.Title));
                    
                })
            );

            return m_searchResultProcessor.ProcessSearchByCriteria(response);
        }

        public FulltextSearchCorpusResultContract SearchCorpusByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var filterQuery =
                m_queriesBuilder.GetFilterSearchQuery(searchRequest.ConditionConjunction, SnapshotIdField);
            var mustQuery = m_queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);


            var client = CommunicationProvider.GetElasticClient();

            var responseCount = client.Search<SnapshotResourceContract>(s => s
                .Index(SnapshotIndex)
                .Type(SnapshotType)
                .From(0)
                .Size(0)
                .Query(q => q
                    .Bool(b => b
                        .Filter(filterQuery)
                        .Must(mustQuery)
                    )
                )
            ).Total;
            return new FulltextSearchCorpusResultContract {Count = responseCount * 30}; //TODO HACK pagination on books
            /*var response = client.Search<SnapshotResourceContract>(s => s
                .Index(SnapshotIndex)
                .Type(SnapshotType)
                .Source(false)
                .Size((int)responseCount)
                .Query(q => q
                    .Bool(b => b
                        .Filter(filterQuery)
                        .Must(mustQuery)
                    )
                )
                .Highlight(h => h
                    .PreTags(HighlightTag)
                    .PostTags(HighlightTag)
                    .Fields(f => f
                        .Field(SnapshotTextField)
                        .NumberOfFragments(FragmentsCount)
                        .FragmentSize(FragmentSize)
                        .Type(HighlighterType)
                    )
                )
            );
            
            return m_searchResultProcessor.ProcessSearchCorpusByCriteriaCount(response, HighlightTag);*/
        }

        public CorpusSearchResultDataList SearchCorpusByCriteria(CorpusSearchRequestContract searchRequest)
        {
            var filterQuery =
                m_queriesBuilder.GetFilterSearchQuery(searchRequest.ConditionConjunction, SnapshotIdField);
            var mustQuery = m_queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);

            var client = CommunicationProvider.GetElasticClient();
            /*
            var responseCount = client.Search<SnapshotResourceContract>(s => s
                .Index(SnapshotIndex)
                .Type(SnapshotType)
                .From(0)
                .Size(0)
                .Query(q => q
                    .Bool(b => b
                        .Filter(filterQuery)
                        .Must(mustQuery)
                    )
                )
            );
            */
            var index = searchRequest.Start / searchRequest.Count;
            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(SnapshotIndex)
                .Type(SnapshotType)
                .Source(sf => sf
                    .IncludeAll()
                    .Excludes(i => i
                        .Fields(
                            f => f.SnapshotText
                        )
                    )
                )
                .Query(q => q
                    .Bool(b => b
                        .Filter(filterQuery)
                        .Must(mustQuery)
                    )
                )
                .From(index ?? DefaultStart)
                .Size(1)
                .Highlight(h => h
                    .PreTags(HighlightTag)
                    .PostTags(HighlightTag)
                    .Fields(f => f
                        .Field(SnapshotTextField)
                        .NumberOfFragments(FragmentsCount)
                        .FragmentSize(FragmentSize)
                        .Type(HighlighterType)
                    )
                )
            );

            return m_searchResultProcessor.ProcessSearchCorpusByCriteria(response, HighlightTag);
        }


        public TextResourceContract SearchPageByCriteria(string textResourceId, SearchPageRequestContract searchRequest)
        {
            var filterQuery = m_queriesBuilder.GetFilterByIdSearchQuery(textResourceId);
            var mustQuery = m_queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, PageTextField);

            var client = CommunicationProvider.GetElasticClient();

            var response = client.Search<TextResourceContract>(s => s
                .Index(PageIndex)
                .Type(PageType)
                .Source(sf => sf.Excludes(e => e.Field(f => f.PageText)))
                .Query(q => q
                    .Bool(b => b
                        .Filter(filterQuery)
                        .Must(mustQuery)
                    )
                )
                .Highlight(h => h
                    .PreTags(EmphTag)
                    .PostTags(EmphTag)
                    .Fields(f => f
                        .Field(PageTextField)
                        .NumberOfFragments(0)
                        .Type(HighlighterType)
                    )
                )
            );

            return m_searchResultProcessor.ProcessSearchPageByCriteria(response);
        }


        public PageSearchResultData SearchPageByCriteria(long snapshotId, SearchRequestContractBase searchRequest)
        {
            var client = CommunicationProvider.GetElasticClient();

            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(SnapshotIndex)
                .Type(SnapshotType)
                .Source(sf => sf.Includes(i => i.Field(f => f.Pages)))
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.SnapshotId)
                        .Value(snapshotId)
                    )
                )
            );

            if (!response.IsValid)
                throw new Exception(response.DebugInformation);

            var pageIdList = response.Documents.SelectMany(document => document.Pages).Select(page => page.Id).ToList();

            var filterQuery = m_queriesBuilder.GetFilterByIdSearchQuery(pageIdList);
            var mustQuery = m_queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, PageTextField);

            var pageResponse = client.Search<TextResourceContract>(s => s
                    .Index(PageIndex)
                    .Type(PageType)
                    .Source(sf => sf.Excludes(i => i.Field(f => f.PageText)))
                    .Query(q => q
                        .Bool(b => b
                            .Filter(filterQuery)
                            .Must(mustQuery)
                        )
                    )
                    .Size(1000) //TODO add pagination
            );
            return m_searchResultProcessor.ProcessSearchPageResult(pageResponse);
        }
    }
}