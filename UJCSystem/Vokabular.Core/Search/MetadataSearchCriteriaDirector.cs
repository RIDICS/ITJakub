using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Search
{
    public class MetadataSearchCriteriaDirector
    {
        private readonly Dictionary<CriteriaKey, ICriteriaImplementationBase> m_criteriaImplementations;

        public MetadataSearchCriteriaDirector(IList<ICriteriaImplementationBase> criteria)
        {
            m_criteriaImplementations = criteria.ToDictionary(x => x.CriteriaKey);
        }

        public SearchCriteriaQuery ProcessCriteria(SearchCriteriaContract searchCriteriaContract, Dictionary<string, object> metadataParameters)
        {
            ICriteriaImplementationBase criteriaImplementation;
            if (!m_criteriaImplementations.TryGetValue(searchCriteriaContract.Key, out criteriaImplementation))
            {
                throw new ArgumentException("Criteria key not found");
            }
            return criteriaImplementation.CreateCriteriaQuery(searchCriteriaContract, metadataParameters);
        }

        public bool IsCriteriaSupported(SearchCriteriaContract searchCriteriaContract)
        {
            var criteriaKey = searchCriteriaContract.Key;
            return m_criteriaImplementations.ContainsKey(criteriaKey);
        }
    }
}
