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
    public class AuthorCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey => CriteriaKey.Author;

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var projectOriginalAuthorAlias = string.Format("poa{0:N}", Guid.NewGuid());
            var originalAuthorAlias = string.Format("oa{0:N}", Guid.NewGuid());

            var whereBuilder = new StringBuilder();
            
            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                var uniqueParameterName = $"param{metadataParameters.Count}";
                whereBuilder.AppendFormat(" concat({0}.FirstName, ' ', {0}.LastName) like (:{1})", originalAuthorAlias, uniqueParameterName);
                whereBuilder.AppendFormat(" or concat({0}.LastName, ' ', {0}.FirstName) like (:{1})", originalAuthorAlias, uniqueParameterName);
                metadataParameters.Add(uniqueParameterName, CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join project.Authors {0} inner join {0}.OriginalAuthor {1}", projectOriginalAuthorAlias, originalAuthorAlias),
                Where = whereBuilder.ToString()
            };
        }
    }
}