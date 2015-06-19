using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core.Search
{
    public class SearchCriteriaDirector
    {
        private readonly Dictionary<CriteriaKey, ICriteriaImplementationBase> m_criteriaImplementations;

        public SearchCriteriaDirector(IKernel container)
        {
            var criteria = container.ResolveAll<ICriteriaImplementationBase>();
            m_criteriaImplementations = criteria.ToDictionary(x => x.CriteriaKey);
        }

        public SearchCriteriaQuery ProcessCriteria(SearchCriteriaContract searchCriteriaContract)
        {
            ICriteriaImplementationBase criteriaImplementation;
            if (!m_criteriaImplementations.TryGetValue(searchCriteriaContract.Key, out criteriaImplementation))
            {
                throw new ArgumentException("Criteria key not found");
            }
            return criteriaImplementation.CreateCriteriaQuery(searchCriteriaContract);
        }
    }
}
