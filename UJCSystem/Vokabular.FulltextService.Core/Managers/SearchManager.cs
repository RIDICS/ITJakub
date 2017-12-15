using System;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.ResultContracts;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SearchManager : ElasticsearchManagerBase
    {
        private const int FragmentSize = 50;
        private const int FragmentsCount = 1000000;
        private const int DefaultStart = 0;
        private const int DefaultSize = 10000;
        private const string HighlightTag = "$";
        
        private const string HighlighterType = "experimental";

        private readonly QueriesBuilder m_queriesBuilder;
        private readonly SearchResultProcessor m_searchResultProcessor;

        public SearchManager(CommunicationProvider communicationProvider, SearchResultProcessor searchResultProcessor, QueriesBuilder queriesBuilder) : base(communicationProvider)
        {
            m_searchResultProcessor = searchResultProcessor;
            m_queriesBuilder = queriesBuilder;
        }
        
        public FulltextSearchResultContract SearchByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var filterQuery = m_queriesBuilder.GetFilterSearchQueryFromRequest(searchRequest, SnapshotIdField);
            var mustQuery = m_queriesBuilder.GetSearchQueryFromRequest(searchRequest, SnapshotTextField);

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

        public FulltextSearchResultContract SearchByCriteria(SearchRequestContractBase searchRequest)
        {
            var filterQuery = m_queriesBuilder.GetFilterSearchQueryFromRequest(searchRequest, SnapshotIdField);
            var mustQuery = m_queriesBuilder.GetSearchQueryFromRequest(searchRequest, SnapshotTextField);


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
            );

            return m_searchResultProcessor.ProcessSearchByCriteria(response);
        }

        public FulltextSearchCorpusResultContract SearchCorpusByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var filterQuery = m_queriesBuilder.GetFilterSearchQueryFromRequest(searchRequest, SnapshotIdField);
            var mustQuery = m_queriesBuilder.GetSearchQueryFromRequest(searchRequest, SnapshotTextField);


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
            return new FulltextSearchCorpusResultContract { Count = responseCount * 30 }; //TODO HACK pagination on books
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
            var filterQuery = m_queriesBuilder.GetFilterSearchQueryFromRequest(searchRequest, SnapshotIdField);
            var mustQuery = m_queriesBuilder.GetSearchQueryFromRequest(searchRequest, SnapshotTextField);

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
            throw new NotImplementedException();
           /* var query = GetHighlightQueryFromRequest(searchRequest);

            var client = CommunicationProvider.GetElasticClient();
            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(Index)
                .Type(SnapshotType)
                .Source(sf => sf
                    .Includes(i => i
                        .Fields(
                            f => f.SnapshotText
                        )
                    )
                )
                .Query(q => q
                    .Bool(b => b
                        .Should(query.ToArray())
                        .Must(m => m.MatchPhrase(mp => mp.Field(IdField).Query(textResourceId)))
                    )
                )
                .Highlight(h => h
                    .PreTags("*")
                    .PostTags("*")
                    .Fields(f => f
                        .Field(PageTextField)
                        .NumberOfFragments(0)
                    )
                )
            );

            if (!response.IsValid)
                throw new Exception(response.DebugInformation);
            foreach (var highlight in response.Hits.Select(d => d.Highlights))
            {
            }
            return null;*/
        }

        
        
        
    }
}