using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.Core.Options;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search.Request;

namespace Vokabular.FulltextService.Core.Managers
{
    public class UnfinishedSearchManager : ElasticsearchManagerBase
    {
        private readonly UnfinishedSearchResultProcessor m_searchResultProcessor;
        private readonly QueriesBuilder m_queriesBuilder;

        public UnfinishedSearchManager(CommunicationProvider communicationProvider, UnfinishedSearchResultProcessor searchResultProcessor,
            IOptions<IndicesOption> indicesOptions) : base(communicationProvider, indicesOptions)
        {
            m_searchResultProcessor = searchResultProcessor;
            m_queriesBuilder = new QueriesBuilder(IndexType.Snapshot);
        }

        public FulltextSearchCorpusResultContract SearchCorpusByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var filterQuery = m_queriesBuilder.GetFilterSearchQuery(searchRequest.ConditionConjunction, SnapshotIdField);
            var mustQuery = m_queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);


            var client = m_communicationProvider.GetElasticClient();

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
            return new FulltextSearchCorpusResultContract { Count = responseCount * 30 }; //TODO pagination on books
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

        public List<CorpusSearchResultContract> SearchCorpusByCriteria(CorpusSearchRequestContract searchRequest)
        {
            var filterQuery =
                m_queriesBuilder.GetFilterSearchQuery(searchRequest.ConditionConjunction, SnapshotIdField);
            var mustQuery = m_queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);

            var client = m_communicationProvider.GetElasticClient();
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
                .From(index ?? SearchManager.DefaultStart)
                .Size(1)
                .Highlight(h => h
                    .PreTags(SearchManager.HighlightTag)
                    .PostTags(SearchManager.HighlightTag)
                    .Fields(f => f
                        .Field(SnapshotTextField)
                        .NumberOfFragments(SearchManager.FragmentsCount)
                        .FragmentSize(SearchManager.FragmentSize)
                        .Type(SearchManager.HighlighterType)
                    )
                )
            );

            return m_searchResultProcessor.ProcessSearchCorpusByCriteria(response, SearchManager.HighlightTag);
        }
    }
}