using System;
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
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Dating; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var datingListCriteriaContract = (DatingListCriteriaContract) searchCriteriaContract;
            var manuscriptAlias = string.Format("m{0}", Guid.NewGuid().ToString("N"));
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

                    var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                    whereBuilder.AppendFormat("{0}.NotAfter >= (:{1})", manuscriptAlias, uniqueParameterName);
                    metadataParameters.Add(uniqueParameterName, datingCriteriaContract.NotBefore.Value);
                }

                if (datingCriteriaContract.NotAfter != null)
                {
                    if (notBeforeUsed)
                    {
                        whereBuilder.Append(" and ");
                    }

                    var uniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
                    whereBuilder.AppendFormat("{0}.NotBefore <= (:{1})", manuscriptAlias,uniqueParameterName);
                    metadataParameters.Add(uniqueParameterName, datingCriteriaContract.NotAfter.Value);
                }
                whereBuilder.Append(")");
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.ManuscriptDescriptions {0}", manuscriptAlias),
                Where = whereBuilder.ToString(),
            };
        }
    }
}