using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class AuthorizationManager
    {
        public void AddAuthorizationCriteria(IList<SearchCriteriaContract> searchCriteriaConjuction)
        {
            if (searchCriteriaConjuction.Any(x => x.Key == CriteriaKey.Authorization))
            {
                throw new ArgumentException("Search criteria contains unallowed Authorization criteria. Authorization criteria is generated automatically.");
            }
            
            //TODO add Authorization criteria to criteria list
        }
    }
}
