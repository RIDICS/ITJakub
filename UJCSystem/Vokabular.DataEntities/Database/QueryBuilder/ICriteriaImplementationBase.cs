using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.QueryBuilder
{
    public interface ICriteriaImplementationBase
    {
        CriteriaKey CriteriaKey { get; }
        SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters);
    }
}
