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
        public CriteriaKey CriteriaKey => CriteriaKey.Term;

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var wordListCriteria = (WordListCriteriaContract)searchCriteriaContract;
            var resourceAlias = string.Format("r{0:N}", Guid.NewGuid());
            var pageResourceAlias = string.Format("pr{0:N}", Guid.NewGuid());
            var termAlias = string.Format("t{0:N}", Guid.NewGuid());
            var whereBuilder = new StringBuilder();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                var uniqueParameterName = $"param{metadataParameters.Count}";
                whereBuilder.AppendFormat(" {0}.Text like (:{1})", termAlias, uniqueParameterName);
                metadataParameters.Add(uniqueParameterName, CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join project.Resources {0} inner join {0}.ResourceVersions {1} inner join {1}.Terms {2}", resourceAlias, pageResourceAlias, termAlias),
                Where = whereBuilder.ToString(),
            };
        }
    }
}