using System;
using System.Collections.Generic;
using System.Text;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.SearchCriteria
{
    public class EditorCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey => CriteriaKey.Editor;

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var projectResponsibleAlias = string.Format("prp{0:N}", Guid.NewGuid());
            var responsiblePersonAlias = string.Format("rp{0:N}", Guid.NewGuid());
            var responsibleTypeAlias = string.Format("rt{0:N}", Guid.NewGuid());
            var whereBuilder = new StringBuilder();

            if (wordListCriteria.Disjunctions.Count > 0)
            {
                whereBuilder.Append('(');

                foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
                {
                    if (whereBuilder.Length > 1)
                        whereBuilder.Append(" or");

                    var uniqueParameterName = $"param{metadataParameters.Count}";
                    whereBuilder.AppendFormat(" concat({0}.FirstName, ' ', {0}.LastName) like (:{1}) ", responsiblePersonAlias, uniqueParameterName);
                    whereBuilder.AppendFormat(" or concat({0}.LastName, ' ', {0}.FirstName) like (:{1}) ", responsiblePersonAlias, uniqueParameterName);
                    metadataParameters.Add(uniqueParameterName, CriteriaConditionBuilder.Create(wordCriteria));
                }

                whereBuilder.Append(')');
            }
            
            if (whereBuilder.Length > 0)
                whereBuilder.Append(" and ");

            var responsibleTypeParameter = $"param{metadataParameters.Count}";
            whereBuilder.AppendFormat("{0}.Type = :{1}", responsibleTypeAlias, responsibleTypeParameter);
            metadataParameters.Add(responsibleTypeParameter, ResponsibleTypeEnum.Editor);

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join project.ResponsiblePersons {0} inner join {0}.ResponsiblePerson {1} inner join {0}.ResponsibleType {2}", projectResponsibleAlias, responsiblePersonAlias, responsibleTypeAlias),
                Where = whereBuilder.ToString(),
            };
        }
    }
}