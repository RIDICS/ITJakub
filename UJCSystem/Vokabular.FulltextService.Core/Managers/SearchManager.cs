using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nest;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SearchManager : ElasticsearchManagerBase
    {
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
                .From(searchRequest.Start ?? 0)
                .Size(searchRequest.Count ?? 10)
                .Source(sf => sf
                    .Includes(i => i
                        .Fields(
                            f => f.SnapshotId
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

            foreach (var document in response.Documents)
                result.ProjectIds.Add(document.SnapshotId);

            return result;
        }

        private QueryContainer GetFilterSearchQueryFromRequest(SearchRequestContractBase searchRequest)
        {
            var filterQueries = GetQueriesFromRequest(searchRequest.ConditionConjunction, QueryType.FilterQuery);
            return filterQueries.FirstOrDefault();
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
            foreach (var criteriaContract in conditionConjunction)
            {
                if (queryType == QueryType.FilterQuery && criteriaContract.Key == CriteriaKey.SnapshotResultRestriction)
                {
                    var snapshotIdFilterQuery = GetSnapshotIdFilterQueryFromCriteria(criteriaContract as SnapshotResultRestrictionCriteriaContract);
                    if (snapshotIdFilterQuery != null)
                    {
                        queryList.Add(snapshotIdFilterQuery);
                    }
                }else if ((queryType == QueryType.SearchQuery || queryType == QueryType.HighlightQuery) && criteriaContract.Key == CriteriaKey.Fulltext)
                {
                    var query = GetShouldQuery(criteriaContract as WordListCriteriaContract, minimumShouldMatch);
                    if (query != null)
                    {
                        queryList.Add(query);
                    }
                }
            }

            return queryList;
        }

        private BoolQuery GetShouldQuery(WordListCriteriaContract worldListCriteriaContract, int minimumShouldMatch = 1)
        {
            if (worldListCriteriaContract == null)
            {
                return null;
            }

            var shouldQueriesList = new List<QueryContainer>();
            foreach (var disjunction in worldListCriteriaContract.Disjunctions)
            {
                shouldQueriesList.Add(new RegexpQuery
                {
                    Field = TextField,
                    Value = GetRegexpFromDisjunction(disjunction),
                    Flags = RegexpQueryFlags,
                });
            }

            return new BoolQuery
            {
                Should = shouldQueriesList,
                MinimumShouldMatch = minimumShouldMatch,
            };
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

        private QueryContainer GetSnapshotIdFilterQueryFromCriteria(SnapshotResultRestrictionCriteriaContract resultRestrictionCriteria)
        {
            return resultRestrictionCriteria == null ? null :
            new QueryContainer(new TermsQuery
            {
                Field = SnapshotIdField,
                Terms = resultRestrictionCriteria.SnapshotIds.Select(id => (object)id), //HACK long to object
            });
        }

        public TextResourceContract SearchPageByCriteria(string textResourceId, SearchPageRequestContract searchRequest)
        {
            var query = GetHighlightQueryFromRequest(searchRequest);

            var client = CommunicationProvider.GetElasticClient();
            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(Index)
                .Type(SnapshotType)
                .Source(sf => sf
                    .Includes(i => i
                        .Fields(
                            f => f.Text
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
                        .Field(TextField)
                        .NumberOfFragments(0)
                    )
                )
            );

            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
            foreach (var highlight in response.Hits.Select(d=>d.Highlights))
            {
                
            }
            return null;
        }
    }

    internal enum QueryType
    {
        FilterQuery,
        SearchQuery,
        HighlightQuery,
    }
}