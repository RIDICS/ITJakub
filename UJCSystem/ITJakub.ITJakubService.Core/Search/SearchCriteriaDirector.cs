using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using ITJakub.ITJakubService.DataContracts;
using NHibernate.Criterion;

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

        public void ProcessCriteria(SearchCriteriaContract searchCriteriaContract, DetachedCriteria databaseCriteria)
        {
            ICriteriaImplementationBase criteriaImplementation;
            if (!m_criteriaImplementations.TryGetValue(searchCriteriaContract.Key, out criteriaImplementation))
            {
                throw new ArgumentException("Criteria key not found");
            }
            criteriaImplementation.ProcessCriteria(searchCriteriaContract, databaseCriteria);
        }
    }
}
