using System.Collections.Generic;
using System.Text;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.SearchCriteria
{
    public class DatingCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey => CriteriaKey.Dating;

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var datingListCriteriaContract = (DatingListCriteriaContract) searchCriteriaContract;

            var whereBuilder = new StringBuilder();

            foreach (DatingCriteriaContract datingCriteriaContract in datingListCriteriaContract.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or ");

                var notBeforeUsed = false;
                whereBuilder.Append("(");

                if (datingCriteriaContract.NotBefore != null)
                {
                    notBeforeUsed = true;

                    var uniqueParameterName = $"param{metadataParameters.Count}";
                    whereBuilder.AppendFormat("metadata.NotAfter >= (:{0})", uniqueParameterName);
                    metadataParameters.Add(uniqueParameterName, datingCriteriaContract.NotBefore.Value);
                }

                if (datingCriteriaContract.NotAfter != null)
                {
                    if (notBeforeUsed)
                    {
                        whereBuilder.Append(" and ");
                    }

                    var uniqueParameterName = $"param{metadataParameters.Count}";
                    whereBuilder.AppendFormat("metadata.NotBefore <= (:{0})", uniqueParameterName);
                    metadataParameters.Add(uniqueParameterName, datingCriteriaContract.NotAfter.Value);
                }
                whereBuilder.Append(")");
            }

            return new SearchCriteriaQuery
            {
                Join = string.Empty,
                Where = whereBuilder.ToString(),
            };
        }
    }
}