using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Managers
{
    public class SearchManager
    {
        private const string Index = "module"; //TODO rename index and type
        private const string Type = "snapshot";

        private readonly CommunicationProvider m_communicationProvider;

        public SearchManager(CommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }


        public FulltextSearchResultContract SearchByCriteria(List<string> searchCriterias, List<long> projectIdList)
        {
            var client = m_communicationProvider.GetElasticClient();
            var queryContainer = new QueryContainer();
            foreach (var searchCriteria in searchCriterias)
            {
                queryContainer |= new MatchPhraseQuery
                {
                    Field = "Text",
                    Query = searchCriteria
                };
                
            }
            var response = client.Search<SnapshotResourceContract>(s => s
                .Index(Index)
                .Type(Type)
                .Source(sf => sf
                    .Includes(i => i
                        .Fields(
                            f => f.ProjectId
                        )
                    )
                )
                .Query(q => q
                    .Bool(b => b
                        .Filter(f => f.Terms(m => m.Field(fi => fi.ProjectId).Terms(projectIdList)))
                        .Should(queryContainer)
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
                result.ProjectIds.Add(document.ProjectId);

            return result;
        }

        /*public FulltextSearchResultContract SearchByCriteria(SearchRequestContractBase searchRequest)
        {
            var snapshotIdFilter = GetSnapshotIdFilterFromRequest(searchRequest);
            List<string> wordList = new List<string>();
            
            foreach (var searchCriteria in searchRequest.ConditionConjunction)
            {
                switch (searchCriteria.Key)
                {
                    case CriteriaKey.Fulltext:
                        var worldListCriteria = searchCriteria as WordListCriteriaContract;
                        if (worldListCriteria == null)
                            continue;
                        foreach (var disjunction in worldListCriteria.Disjunctions)
                        {
                            foreach (var contain in disjunction.Contains)
                            {
                                wordList.Add(contain);
                            }
                        }
                        break;
                    
                }
            }
        }

        /*private QueryContainer GetSearchQueryFromRequest(SearchRequestContract searchRequest)
        {
            
        }
        */
        private QueryContainer GetSnapshotIdFilterFromRequest(SearchRequestContractBase searchRequest)
        {
            var snapshotIdList = new List<long>();
            foreach (var criteriaContract in searchRequest.ConditionConjunction)
            {
                if (criteriaContract.Key != CriteriaKey.SnapshotResultRestriction)
                {
                    continue;
                }
                var resultRestrictionCriteria = criteriaContract as SnapshotResultRestrictionCriteriaContract;
                if (resultRestrictionCriteria == null)
                {
                    continue;
                }
                snapshotIdList.AddRange(resultRestrictionCriteria.SnapshotIds);
            }

            QueryContainer queryContainer = new QueryContainer(new TermsQuery
            {
                Field = "ProjectId",
                Terms = snapshotIdList.Select(x => (object)x), //HACK long to object
            });

            return queryContainer;
        }
    }
}