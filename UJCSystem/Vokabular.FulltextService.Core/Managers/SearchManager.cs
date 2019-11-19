using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.Core.Options;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SearchManager : ElasticsearchManagerBase
    {
        public const int FragmentSize = 50;
        public const int FragmentsCount = 1000000;
        public const int DefaultStart = 0;
        public const int DefaultSize = 10000;
        public const int BatchSize = 5;
        public const string HighlightTag = "¼";
        public const string OpeningEmphTag = "<span class=\"reader-search-result-match\">";
        public const string ClosingEmphTag = "</span>";
        public const string HighlighterType = "experimental";

        private readonly SearchResultProcessor m_searchResultProcessor;
        private readonly QueriesBuilderFactory m_queriesBuilderFactory;


        public SearchManager(CommunicationProvider communicationProvider, SearchResultProcessor searchResultProcessor,
            QueriesBuilderFactory queriesBuilderFactory, IOptions<IndicesOption> indicesOptions) : base(communicationProvider, indicesOptions)
        {
            m_searchResultProcessor = searchResultProcessor;
            m_queriesBuilderFactory = queriesBuilderFactory;
        }

        public FulltextSearchResultContract SearchProjectsByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var queriesBuilder = m_queriesBuilderFactory.Create(IndexType.Snapshot);
            var filterQuery =
                queriesBuilder.GetFilterSearchQuery(searchRequest.ConditionConjunction, SnapshotIdField);
            var mustQuery = queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);

            var client = m_communicationProvider.GetElasticClient();

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

        public FulltextSearchResultContract SearchProjectsByCriteria(SearchRequestContract searchRequest)
        {
            var queriesBuilder = m_queriesBuilderFactory.Create(IndexType.Snapshot);
            var filterQuery =
                queriesBuilder.GetFilterSearchQuery(searchRequest.ConditionConjunction, SnapshotIdField);
            var mustQuery = queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);


            var client = m_communicationProvider.GetElasticClient();

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

        public TextResourceContract SearchOnPageByCriteria(string textResourceId, SearchPageRequestContract searchRequest)
        {
            var queriesBuilder = m_queriesBuilderFactory.Create(IndexType.Page);
            var filterQuery = queriesBuilder.GetFilterByIdSearchQuery(textResourceId);
            var mustQuery = queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, PageTextField);

            var client = m_communicationProvider.GetElasticClient();

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
                    .PreTags(OpeningEmphTag)
                    .PostTags(ClosingEmphTag)
                    .Fields(f => f
                        .Field(PageTextField)
                        .NumberOfFragments(0)
                        .Type(HighlighterType)
                    )
                )
            );

            return m_searchResultProcessor.ProcessSearchPageByCriteria(response);
        }


        public PageSearchResultContract SearchPageByCriteria(long snapshotId, SearchPageRequestContract searchRequest)
        {
            var pageIdList = GetPageIds(snapshotId);

            var client = m_communicationProvider.GetElasticClient();

            var queriesBuilder = m_queriesBuilderFactory.Create(IndexType.Page);
            var filterQuery = queriesBuilder.GetFilterByIdSearchQuery(pageIdList);
            var mustQuery = queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, PageTextField);

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
                    .Size(DefaultSize) //WORKAROUND we need all pages which have any hits, so specify big enough number
            );
            return m_searchResultProcessor.ProcessSearchPageResult(pageResponse);
        }

        public long SearchHitsResultCount(long snapshotId, SearchPageRequestContract searchRequest)
        {
            var pageIdList = GetPageIds(snapshotId);

            var client = m_communicationProvider.GetElasticClient();

            var queriesBuilder = m_queriesBuilderFactory.Create(IndexType.Page);
            var filterQuery = queriesBuilder.GetFilterByIdSearchQuery(pageIdList);
            var mustQuery = queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, PageTextField);

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
                    .Highlight(h => h
                        .PreTags(HighlightTag)
                        .PostTags(HighlightTag)
                        .Fields(f => f
                            .Field(PageTextField)
                            .NumberOfFragments(FragmentsCount)
                            .FragmentSize(FragmentSize)
                            .Type(HighlighterType)
                        )
                    )
                    .Size(DefaultSize) //WORKAROUND we need get all hits to sum total hit count, so specify big enough number
            );
            return m_searchResultProcessor.ProcessSearchPageResultCount(pageResponse, HighlightTag);
        }

        private List<string> GetPageIds(long snapshotId)
        {
            var client = m_communicationProvider.GetElasticClient();

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
                throw new FulltextDatabaseException(response.DebugInformation);

            return response.Documents.SelectMany(document => document.Pages).Select(page => page.Id).ToList();
        }

        private List<SnapshotPageResourceContract> GetSnapshotPages(long snapshotId)
        {
            var client = m_communicationProvider.GetElasticClient();

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
                throw new FulltextDatabaseException(response.DebugInformation);

            return response.Documents.SelectMany(document => document.Pages).ToList();
        }

        public CorpusSearchSnapshotsResultContract SearchCorpusSnapshotsByCriteria(BookPagedCorpusSearchRequestContract searchRequest)
        {
            var queriesBuilder = m_queriesBuilderFactory.Create(IndexType.Snapshot);
            var filterQuery = queriesBuilder.GetFilterSearchQuery(searchRequest.ConditionConjunction, SnapshotIdField);
            var mustQuery = queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);

            var client = m_communicationProvider.GetElasticClient();

            if (searchRequest.FetchNumberOfResults)
            {
                var response = client.Search<SnapshotResourceContract>(s => s
                    .Index(SnapshotIndex)
                    .Type(SnapshotType)
                    .Source(sf => sf.Includes(i => i.Field(f => f.SnapshotId)))
                    .Query(q => q
                        .Bool(b => b
                            .Filter(filterQuery)
                            .Must(mustQuery)
                        )
                    )
                    .From(searchRequest.Start ?? DefaultStart)
                    .Size(searchRequest.Count ?? DefaultSize)
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

                return m_searchResultProcessor.ProcessSearchCorpusSnapshotsByCriteriaFetchResultCount(response, HighlightTag);

            }
            else
            {

                var response = client.Search<SnapshotResourceContract>(s => s
                    .Index(SnapshotIndex)
                    .Type(SnapshotType)
                    .Source(sf => sf.Includes(i => i.Field(f => f.SnapshotId)))
                    .Query(q => q
                        .Bool(b => b
                            .Filter(filterQuery)
                            .Must(mustQuery)
                        )
                    )
                    .From(searchRequest.Start ?? DefaultStart)
                    .Size(searchRequest.Count ?? DefaultSize)
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

                return m_searchResultProcessor.ProcessSearchCorpusSnapshotsByCriteria(response);
            }
        }

        public List<CorpusSearchResultContract> SearchCorpusSnapshotByCriteria(long snapshotId, BookPagedCorpusSearchInSnapshotRequestContract searchRequest)
        {
            var queriesBuilder = m_queriesBuilderFactory.Create(IndexType.Snapshot);
            var mustQuery = queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);
            var filterQuery = queriesBuilder.GetFilterByFieldSearchQuery(SnapshotIdField, snapshotId.ToString());

            var client = m_communicationProvider.GetElasticClient();


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
                .Highlight(h => h
                    .PreTags(HighlightTag)
                    .PostTags(HighlightTag)
                    .Fields(f => f
                        .Field(SnapshotTextField)
                        .NumberOfFragments(FragmentsCount)
                        .FragmentSize(searchRequest.ContextLength)
                        .Type(HighlighterType)
                    )
                )
            );

            return m_searchResultProcessor.ProcessSearchCorpusSnapshotByCriteria(response, HighlightTag, searchRequest.Start ?? DefaultStart, searchRequest.Count ?? DefaultSize);
        }

        public async Task<long> SearchCorpusSnapshotsByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var queriesBuilder = m_queriesBuilderFactory.Create(IndexType.Snapshot);
            var filterQuery = queriesBuilder.GetFilterSearchQuery(searchRequest.ConditionConjunction, SnapshotIdField);
            var mustQuery = queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, SnapshotTextField);

            var client = m_communicationProvider.GetElasticClient();
            List<Task<FulltextSearchCorpusResultContract>> tasks = new List<Task<FulltextSearchCorpusResultContract>>();
            int numberOfHits;
            int loopCounter = 0;
            do
            {
                var response = client.Search<SnapshotResourceContract>(s => s
                    .Index(SnapshotIndex)
                    .Type(SnapshotType)
                    .Source(false)
                    .Size(BatchSize)
                    .From(loopCounter++ * BatchSize)
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

                numberOfHits = response.Hits.Count;
                
                if (numberOfHits == 0)
                {
                    break;
                }

                tasks.Add(Task<FulltextSearchCorpusResultContract>.Factory.StartNew(() => m_searchResultProcessor.ProcessSearchCorpusByCriteriaCount(response, HighlightTag)));

            } while (numberOfHits == BatchSize);

            long counter = 0;
            await Task.WhenAll(tasks).ContinueWith(newTask =>
            {
                foreach (var task in tasks)
                {
                    counter += task.Result.Count;
                }
            });

            return counter;
        }


        public HitsWithPageContextResultContract SearchHitsWithPageContext(long snapshotId, SearchHitsRequestContract searchRequest)
        {
            var pageList = GetSnapshotPages(snapshotId);

            var client = m_communicationProvider.GetElasticClient();

            var queriesBuilder = m_queriesBuilderFactory.Create(IndexType.Page);
            var filterQuery = queriesBuilder.GetFilterByIdSearchQuery(pageList.Select(x => x.Id).ToList());
            var mustQuery = queriesBuilder.GetSearchQuery(searchRequest.ConditionConjunction, PageTextField);

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
                    .Highlight(h => h
                        .PreTags(HighlightTag)
                        .PostTags(HighlightTag)
                        .Fields(f => f
                            .Field(PageTextField)
                            .NumberOfFragments(FragmentsCount)
                            .FragmentSize(searchRequest.ContextLength)
                            .Type(HighlighterType)
                        )
                    )
                    .Size(DefaultSize) //WORKAROUND get all hits and create paging manually, so specify big enough number
            );
            return m_searchResultProcessor.ProcessSearchHitsWithPageContext(pageResponse, pageList, HighlightTag, searchRequest.Start ?? DefaultStart, searchRequest.Count ?? DefaultSize);
        }
    }
}