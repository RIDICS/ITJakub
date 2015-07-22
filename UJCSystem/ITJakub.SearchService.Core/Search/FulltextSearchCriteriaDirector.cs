using System.Collections.Generic;
using ITJakub.Shared.Contracts.Searching.Criteria;

namespace ITJakub.SearchService.Core.Search
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
                CriteriaKey.Sentence,
                CriteriaKey.TokenDistance,
                CriteriaKey.Headword,
                CriteriaKey.HeadwordDescription,
                CriteriaKey.HeadwordDescriptionTokenDistance
            };
        }

        public bool IsCriteriaSupported(SearchCriteriaContract searchCriteriaContract)
        {
            var criteriaKey = searchCriteriaContract.Key;
            return m_supportedCriteriaSet.Contains(criteriaKey);
        }
    }
}
