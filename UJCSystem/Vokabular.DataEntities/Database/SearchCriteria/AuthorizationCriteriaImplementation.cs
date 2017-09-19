using System;
using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.SearchCriteria
{
    public class AuthorizationCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Authorization; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            throw new NotSupportedException("Permissions are currently not supported");
            //var authorizationCriteria = (AuthorizationCriteriaContract)searchCriteriaContract;

            //var bookAlias = string.Format("ba{0}", Guid.NewGuid().ToString("N"));
            //var permissionAlias = string.Format("pa{0}", Guid.NewGuid().ToString("N"));
            //var groupAlias = string.Format("ga{0}", Guid.NewGuid().ToString("N"));
            //var userAlias = string.Format("ua{0}", Guid.NewGuid().ToString("N"));

            //var userUniqueParameterName = string.Format("up{0}", Guid.NewGuid().ToString("N"));
            //metadataParameters.Add(userUniqueParameterName, authorizationCriteria.UserId);
          
            //return new SearchCriteriaQuery
            //{
            //    Join = string.Format("inner join bv.Book {0} inner join {0}.Permissions {1} inner join {1}.Group {2} inner join {2}.Users {3}", bookAlias, permissionAlias, groupAlias, userAlias),
            //    Where = string.Format("{0}.Id = (:{1})", userAlias, userUniqueParameterName),
            //};
        }
    }
}