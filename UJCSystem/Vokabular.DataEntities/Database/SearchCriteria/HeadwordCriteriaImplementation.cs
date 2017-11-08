using System.Collections.Generic;
using NHibernate.Criterion;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.SearchCriteria
{
    public class HeadwordCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey => CriteriaKey.Headword;

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            HeadwordItem headwordItemAlias = null;
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var disjunction = new Disjunction();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                var parameter = CriteriaConditionBuilder.Create(wordCriteria);
                var restriction = Restrictions.Like(Projections.Property(() => headwordItemAlias.Headword), parameter, MatchMode.Exact);

                disjunction.Add(restriction);
            }

            return new SearchCriteriaQuery
            {
                CriteriaKey = CriteriaKey,
                Join = string.Empty,
                Where = string.Empty,
                Restriction = disjunction,
            };
        }
    }
}