using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nest;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.Shared.DataContracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.ResultContracts;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SearchManager : ElasticsearchManagerBase
    {
        private const int FragmentSize = 50;
        private const int FragmentNumber = 10000;
        private const int DefaultStart = 0;
        private const int DefaultSize = 10;
        private const string HighlightTag = "$";
        
        public SearchManager(CommunicationProvider communicationProvider) : base(communicationProvider){}

   
        public FulltextSearchResultContract SearchByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var filterQuery = GetFilterSearchQueryFromRequest(searchRequest);
            var mustQuery = GetMustSearchQueryFromRequest(searchRequest);

            var client = CommunicationProvider.GetElasticClient();

            var response = client.Count<SnapshotResourceContract>(s => s
                .Index(Index)
                .Type(SnapshotType)
                .Query(q => q
                    .Bool(b => b
                        .Filter(filterQuery)
                        .Must(mustQuery.ToArray())
                    )
                )
            );

            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            return new FulltextSearchResultContract{Count = response.Count};
        }
        public FulltextSearchResultContract SearchByCriteria(SearchRequestContractBase searchRequest)
        {
            var filterQuery = GetFilterSearchQueryFromRequest(searchRequest);
            var mustQuery = GetMustSearchQueryFromRequest(searchRequest);
            
            var client = CommunicationProvider.GetElasticClient();

            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(Index)
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
                        .Must(mustQuery.ToArray())
                    )
                )
            );
            
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }

            var result = new FulltextSearchResultContract();
            result.ProjectIds = new List<long>();
            result.ProjectIds.AddRange(response.Documents.Select(d => d.ProjectId));
            
            return result;
        }

        public FulltextSearchCorpusResultContract SearchCorpusByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var filterQuery = GetFilterSearchQueryFromRequest(searchRequest);
            var mustQuery = GetMustSearchQueryFromRequest(searchRequest);

            var client = CommunicationProvider.GetElasticClient();

            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(Index)
                .Type(SnapshotType)
                .Source(sf => sf
                    .Includes(i => i
                        .Fields(
                            f => null
                        )
                    )
                )
                .Query(q => q
                    .Bool(b => b
                        .Filter(filterQuery)
                        .Must(mustQuery.ToArray())
                    )
                )
                .Highlight(h => h
                    .PreTags(HighlightTag)
                    .PostTags(HighlightTag)
                    .Fields(f => f
                        .Field(SnapshotTextField)
                        .NumberOfFragments(FragmentNumber)
                        .FragmentSize(FragmentSize)
                        .Type(HighlighterType.Fvh)
                        )
                )
                
            );

            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
            int counter = 0;
            foreach (var highlightField in response.Hits.Select(d => d.Highlights))
            {
                foreach (var value in highlightField.Values)
                {
                    foreach (var highlight in value.Highlights)
                    {
                        counter += GetNumberOfHighlitOccurences(highlight);
                    }
                }
            }
            return new FulltextSearchCorpusResultContract{ Count = counter };
        }

        public CorpusSearchResultDataList SearchCorpusByCriteria(CorpusSearchRequestContract searchRequest)
        {
            var start = searchRequest.Start ?? 0;
            var count = searchRequest.Count ?? 10;
            var filterQuery = GetFilterSearchQueryFromRequest(searchRequest);
            var mustQuery = GetMustSearchQueryFromRequest(searchRequest);

            var client = CommunicationProvider.GetElasticClient();

            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(Index)
                .Type(SnapshotType)
                .Source(sf => sf
                    .Includes(i => i
                        .Fields(
                            f => null
                        )
                    )
                )
                .Query(q => q
                    .Bool(b => b
                        .Filter(filterQuery)
                        .Must(mustQuery.ToArray())
                    )
                )
                .Highlight(h => h
                    .PreTags(HighlightTag)
                    .PostTags(HighlightTag)
                    .Fields(f => f
                        .Field(SnapshotTextField)
                        .NumberOfFragments(FragmentNumber)
                        .FragmentSize(FragmentSize)
                        .Type(HighlighterType.Fvh)
                    )
                )

            );

            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
            int startCounter = 0;
            var result = new CorpusSearchResultDataList {List = new List<CorpusSearchResultData>(),  SearchResultType = FulltextSearchResultType.ProjectExternalId};
            
            foreach (var highlightField in response.Hits.Select(d => d.Highlights))
            {
                foreach (var value in highlightField.Values)
                {
                    foreach (var highlight in value.Highlights)
                    {
                        var numberOfOccurences = GetNumberOfHighlitOccurences(highlight);

                        if (startCounter + numberOfOccurences <= start)
                        {
                            startCounter += numberOfOccurences;
                            continue;
                        }

                        var resultData = GetCorpusSearchResultDataList(highlight, value.DocumentId);

                        if (startCounter < start)
                        {
                            resultData.RemoveRange(0, start - startCounter);
                            startCounter += resultData.Count;
                        }

                        if (result.List.Count + resultData.Count > count)
                        {
                            resultData = resultData.GetRange(0, count - result.List.Count);
                        }

                        result.List.AddRange(resultData);
                        
                        if (result.List.Count == count)
                        {
                            return result;
                        }
                    }
                }
            }
            return result;
        }

        public TextResourceContract SearchPageByCriteria(string textResourceId, SearchPageRequestContract searchRequest)
        {
            throw new NotImplementedException();
            var query = GetHighlightQueryFromRequest(searchRequest);

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
            {
                throw new Exception(response.DebugInformation);
            }
            foreach (var highlight in response.Hits.Select(d => d.Highlights))
            {

            }
            return null;
        }

        private QueryContainer GetFilterSearchQueryFromRequest(SearchRequestContractBase searchRequest)
        {
            return GetQueriesFromRequest(searchRequest.ConditionConjunction, QueryType.FilterQuery).FirstOrDefault();
        }

        private IEnumerable<QueryContainer> GetMustSearchQueryFromRequest(SearchRequestContractBase searchRequest)
        {
            return GetQueriesFromRequest(searchRequest.ConditionConjunction, QueryType.SearchQuery);
        }

        private IEnumerable<QueryContainer> GetHighlightQueryFromRequest(SearchPageRequestContract searchRequest)
        {
            return GetQueriesFromRequest(searchRequest.ConditionConjunction, QueryType.HighlightQuery);
        }

        private IEnumerable<QueryContainer> GetQueriesFromRequest(IList<SearchCriteriaContract> conditionConjunction, QueryType queryType)
        {
            var queryList = new List<QueryContainer>();
            var minimumShouldMatch = queryType == QueryType.SearchQuery ? 1 : 0;

            foreach (var searchCriteria in conditionConjunction)
            {
                if (queryType == QueryType.FilterQuery && searchCriteria.Key == CriteriaKey.SnapshotResultRestriction)
                {
                    var snapshotIdFilterQuery = GetSnapshotIdFilterQueryFromCriteria(searchCriteria as SnapshotResultRestrictionCriteriaContract);

                    if (snapshotIdFilterQuery != null)
                    {
                        queryList.Add(snapshotIdFilterQuery);
                    }
                }
                else if ((queryType == QueryType.SearchQuery || queryType == QueryType.HighlightQuery) && searchCriteria.Key == CriteriaKey.Fulltext)
                {
                    var query = GetBoolShouldQuery(searchCriteria as WordListCriteriaContract, minimumShouldMatch);
                    if (query != null)
                    {
                        queryList.Add(query);
                    }
                }
            }

            return queryList;
        }

        private BoolQuery GetBoolShouldQuery(WordListCriteriaContract worldListCriteriaContract, int minimumShouldMatch = 1)
        {
            if (worldListCriteriaContract == null)
            {
                return null;
            }

            var shouldQueryList = new List<QueryContainer>();

            foreach (var disjunction in worldListCriteriaContract.Disjunctions)
            {
                shouldQueryList.Add(new RegexpQuery
                {
                    Field = SnapshotTextField,
                    Value = GetRegexpFromDisjunction(disjunction),
                    Flags = RegexpQueryFlags,
                });
            }

            return new BoolQuery
            {
                Should = shouldQueryList,
                MinimumShouldMatch = minimumShouldMatch,
            };
        }

        private QueryContainer GetSnapshotIdFilterQueryFromCriteria(SnapshotResultRestrictionCriteriaContract resultRestrictionCriteria)
        {
            return resultRestrictionCriteria == null ? null :
                new QueryContainer(new TermsQuery
                {
                    Field = SnapshotIdField,
                    Terms = resultRestrictionCriteria.SnapshotIds.Select(id => (object)id), //HACK long to object
                });
        }

        private string GetRegexpFromDisjunction(WordCriteriaContract disjunction)
        {
            if (!string.IsNullOrWhiteSpace(disjunction.ExactMatch))
            {
                return disjunction.ExactMatch;
            }

            var regexpBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(disjunction.StartsWith))
            {
                regexpBuilder.Append(disjunction.StartsWith.ToLower());
            }

            regexpBuilder.Append(".*");

            foreach (var contain in disjunction.Contains)
            {
                if (!string.IsNullOrWhiteSpace(contain))
                {
                    regexpBuilder.Append(contain.ToLower());
                    regexpBuilder.Append(".*");
                }
            }

            if (!string.IsNullOrWhiteSpace(disjunction.EndsWith))
            {
                regexpBuilder.Append(disjunction.EndsWith.ToLower());
            }

            return regexpBuilder.ToString();
        }
        
        private int GetNumberOfHighlitOccurences(string highlightedText)
        {
            return highlightedText.Split(new[] { HighlightTag }, StringSplitOptions.None).Length / 2;
        }

        private List<CorpusSearchResultData> GetCorpusSearchResultDataList(string highlightedText, string externalId)
        {
            var result = new List<CorpusSearchResultData>();

            var index = highlightedText.IndexOf(HighlightTag, StringComparison.Ordinal);

            do
            {
                var corpusSearchResult = GetCorpusSearchResultData(highlightedText, externalId, index, out index);
                result.Add(corpusSearchResult);

                index = highlightedText.IndexOf(HighlightTag, index + HighlightTag.Length, StringComparison.Ordinal);

            } while (index > 0);

            return result;
        }

        private CorpusSearchResultData GetCorpusSearchResultData(string highlightedText, string externalId, int index, out int newIndex)
        {
            newIndex = highlightedText.IndexOf(HighlightTag, index + 1, StringComparison.Ordinal);

            var before = highlightedText.Substring(0, index);
            var match = highlightedText.Substring(index + HighlightTag.Length, newIndex - (index + HighlightTag.Length));
            var after = highlightedText.Substring(newIndex + HighlightTag.Length, highlightedText.Length - (newIndex + HighlightTag.Length));

            before = before.Replace(HighlightTag, "");
            after = after.Replace(HighlightTag, "");

            return new CorpusSearchResultData { ProjectExternalId = externalId, PageResultContext = new CorpusSearchPageResultData { ContextStructure = new KwicStructure { After = after, Before = before, Match = match } } };
        }
    }

    internal enum QueryType
    {
        FilterQuery,
        SearchQuery,
        HighlightQuery,
    }
}