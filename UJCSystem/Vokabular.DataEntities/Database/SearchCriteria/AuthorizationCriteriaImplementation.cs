using System;
using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.SearchCriteria
{
    public class AuthorizationCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey => CriteriaKey.Authorization;

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            var authorizationCriteria = (AuthorizationCriteriaContract) searchCriteriaContract;

            var permissionAlias = string.Format("p{0:N}", Guid.NewGuid());
            var userGroupAlias = string.Format("ug{0:N}", Guid.NewGuid());
            var userAlias = string.Format("u{0:N}", Guid.NewGuid());

            var uniqueParameterName = $"param{metadataParameters.Count}";
            metadataParameters.Add(uniqueParameterName, authorizationCriteria.UserId);

            return new SearchCriteriaQuery
            {
                CriteriaKey = CriteriaKey,
                Join = string.Format("inner join project.Permissions {0} inner join {0}.UserGroup {1} inner join {1}.Users {2}", permissionAlias, userGroupAlias, userAlias),
                Where = $"{userAlias}.Id = (:{uniqueParameterName})",
            };
        }
    }
}