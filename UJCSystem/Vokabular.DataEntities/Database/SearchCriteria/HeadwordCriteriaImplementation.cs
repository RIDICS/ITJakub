using System;
using System.Collections.Generic;
using System.Text;
using Vokabular.DataEntities.Database.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.SearchCriteria
{
    public class HeadwordCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Headword; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var bookHeadwordAlias = string.Format("bh{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                {
                    whereBuilder.Append(" or");
                }

                var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                whereBuilder.AppendFormat(" {0}.Headword like (:{1})", bookHeadwordAlias, uniqueParameterName);
                metadataParameters.Add(uniqueParameterName, CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.BookHeadwords {0}", bookHeadwordAlias),
                Where = whereBuilder.ToString()
            };
        }
    }
}