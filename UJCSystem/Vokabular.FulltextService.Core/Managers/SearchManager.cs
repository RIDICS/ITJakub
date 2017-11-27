using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nest;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Helpers;
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
        private const int FragmentNumber = 1000000;
        private const int DefaultStart = 0;
        private const int DefaultSize = 10;
        private const string HighlightTag = "$";
        private const string ReservedChars = ".?+*|{}[]()\"\\#@&<>~";

        private readonly SearchResultProcessor m_searchResultProcessor;

        public SearchManager(CommunicationProvider communicationProvider, SearchResultProcessor searchResultProcessor) : base(communicationProvider)
        {
            m_searchResultProcessor = searchResultProcessor;
        }
        
        public FulltextSearchResultContract SearchByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var filterQuery = GetFilterSearchQueryFromRequest(searchRequest);
            var mustQuery = GetSearchQueryFromRequest(searchRequest);

            var client = CommunicationProvider.GetElasticClient();

            var response = client.Count<SnapshotResourceContract>(s => s
                .Index(Index)
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
            var filterQuery = GetFilterSearchQueryFromRequest(searchRequest);
            var mustQuery = GetSearchQueryFromRequest(searchRequest);

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
                        .Must(mustQuery)
                    )
                )
            );

            return m_searchResultProcessor.ProcessSearchByCriteria(response);
        }

        public FulltextSearchCorpusResultContract SearchCorpusByCriteriaCount(SearchRequestContractBase searchRequest)
        {
            var filterQuery = GetFilterSearchQueryFromRequest(searchRequest);
            var mustQuery = GetSearchQueryFromRequest(searchRequest);

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
                        .Must(mustQuery)
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

            return m_searchResultProcessor.ProcessSearchCorpusByCriteriaCount(response, HighlightTag);
        }

        public CorpusSearchResultDataList SearchCorpusByCriteria(CorpusSearchRequestContract searchRequest)
        {
            var filterQuery = GetFilterSearchQueryFromRequest(searchRequest);
            var mustQuery = GetSearchQueryFromRequest(searchRequest);

            var client = CommunicationProvider.GetElasticClient();

            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(Index)
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
                        .NumberOfFragments(FragmentNumber)
                        .FragmentSize(FragmentSize)
                        .Type(HighlighterType.Fvh)
                    )
                )
            );

            return m_searchResultProcessor.ProcessSearchCorpusByCriteria(response, HighlightTag, searchRequest.Start ?? 0, searchRequest.Count ?? 10);
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

        private QueryContainer GetFilterSearchQueryFromRequest(SearchRequestContractBase searchRequest)
        {
            var snapshotRestrictions = new List<SnapshotResultRestrictionCriteriaContract>();
            foreach (var conjunction in searchRequest.ConditionConjunction)
            {
                if (conjunction.Key == CriteriaKey.SnapshotResultRestriction)
                {
                    snapshotRestrictions.Add(conjunction as SnapshotResultRestrictionCriteriaContract);
                }
            }
            return GetSnapshotIdFilterQuery(snapshotRestrictions);
        }

        private QueryContainer GetSearchQueryFromRequest(SearchRequestContractBase searchRequest)
        {
            StringBuilder regexBuilder = new StringBuilder();

            foreach (var conjunction in searchRequest.ConditionConjunction)
            {
                if (conjunction.Key == CriteriaKey.Fulltext)
                {
                    var regex = GetRegexFromWordList(conjunction as WordListCriteriaContract);

                    regexBuilder.Append("(");
                    regexBuilder.Append(regex);
                    regexBuilder.Append(")&");
                }
            }

            regexBuilder.Length--;
            return new RegexpQuery
            {
                Field = SnapshotTextField,
                Value = regexBuilder.ToString(),
                Flags = RegexpQueryFlags
            };
        }

        private string GetRegexFromWordList(WordListCriteriaContract wordListCriteriaContract)
        {
            if (wordListCriteriaContract == null)
            {
                return null;
            }

            StringBuilder regexBuilder = new StringBuilder();

            foreach (var disjunction in wordListCriteriaContract.Disjunctions)
            {
                regexBuilder.Append("(");
                regexBuilder.Append(GetRegexFromDisjunction(disjunction));
                regexBuilder.Append(")|");
            }

            regexBuilder.Length--;
            return regexBuilder.ToString();
        }
        
        private QueryContainer GetSnapshotIdFilterQuery(List<SnapshotResultRestrictionCriteriaContract> snapshotRestrictions)
        {
            var idList = new List<object>();
            foreach (var restriction in snapshotRestrictions)
            {
                if (restriction != null && restriction.SnapshotIds != null)
                {
                    idList.AddRange(restriction.SnapshotIds.Select(id => (object) id)); //HACK long to object
                }
            }
            return new QueryContainer(new TermsQuery
            {
                Field = SnapshotIdField,
                Terms = idList,
            });
        }

        private string GetRegexFromDisjunction(WordCriteriaContract disjunction)
        {
            if (!string.IsNullOrWhiteSpace(disjunction.ExactMatch))
                return EscapeChars(disjunction.ExactMatch);

            var regexBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(disjunction.StartsWith))
            {
                regexBuilder.Append(disjunction.StartsWith.ToLower());
            }

            regexBuilder.Append(".*");

            foreach (var contain in disjunction.Contains)
            {
                if (!string.IsNullOrWhiteSpace(contain))
                {
                    var escapedText = EscapeChars(contain.ToLower());
                    regexBuilder.Append(escapedText);
                    regexBuilder.Append(".*");
                }
            }

            if (!string.IsNullOrWhiteSpace(disjunction.EndsWith))
            {
                regexBuilder.Append(disjunction.EndsWith.ToLower());
            }

            return regexBuilder.ToString();
        }

        private string EscapeChars(string text)
        {
            return text; //TODO 
            foreach (var reservedChar in ReservedChars)
            {
                text = text.Replace(reservedChar.ToString(), $"\\{reservedChar}");
            }

            return text;
        }

        
    }
}