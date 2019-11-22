using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        private readonly CriteriaKey[] m_supportedCriteriaKeys = new[]
        {
            CriteriaKey.SnapshotResultRestriction,
            CriteriaKey.Fulltext,
            CriteriaKey.Heading,
            CriteriaKey.Sentence,
            CriteriaKey.Headword,
            CriteriaKey.HeadwordDescription,
            CriteriaKey.TokenDistance,
            CriteriaKey.HeadwordDescriptionTokenDistance,
        };

        protected bool ContainsAnyUnsupportedCriteria(SearchRequestContractBase request)
        {
            return ContainsAnyUnsupportedCriteria(request.ConditionConjunction);
        }

        protected bool ContainsAnyUnsupportedCriteria(SearchPageRequestContract request)
        {
            return ContainsAnyUnsupportedCriteria(request.ConditionConjunction);
        }

        private bool ContainsAnyUnsupportedCriteria(IList<SearchCriteriaContract> conditionConjunction)
        {
            return conditionConjunction.Any(x => !m_supportedCriteriaKeys.Contains(x.Key));
        }
    }
}
