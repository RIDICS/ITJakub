using System.Collections.Generic;
using ITJakub.Shared.Contracts.Searching;

namespace ITJakub.SearchService.Core.Exist
{
    public class FulltextSearchCriteriaDirector
    {
        private readonly HashSet<CriteriaKey> m_supportedCriteriaSet;

        public FulltextSearchCriteriaDirector()
        {
            m_supportedCriteriaSet = new HashSet<CriteriaKey>
            {
                CriteriaKey.Fulltext,
                CriteriaKey.Heading,
                CriteriaKey.Sentence
            };
        }

        public bool IsCriteriaSupported(SearchCriteriaContract searchCriteriaContract)
        {
            var criteriaKey = searchCriteriaContract.Key;
            return m_supportedCriteriaSet.Contains(criteriaKey);
        }
    }
}
