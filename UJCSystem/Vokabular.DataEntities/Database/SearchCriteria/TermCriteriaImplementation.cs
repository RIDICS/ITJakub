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
    public class TermCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Term; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var wordListCriteria = (WordListCriteriaContract)searchCriteriaContract;
            var pageAlias = string.Format("pa{0}", Guid.NewGuid().ToString("N"));
            var termAlias = string.Format("ta{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                whereBuilder.AppendFormat(" {0}.Text like (:{1})", termAlias, uniqueParameterName);
                metadataParameters.Add(uniqueParameterName, CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.BookPages {0} inner join {0}.Terms {1}", pageAlias, termAlias),
                Where = whereBuilder.ToString(),
            };
        }
    }
}