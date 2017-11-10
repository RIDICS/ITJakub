using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<SearchController>();

        private readonly SearchManager m_searchManager;

        public SearchController(SearchManager searchManager)
        {
            m_searchManager = searchManager;
        }

        [HttpPost]
        public FulltextSearchResultContract SearchByCriteria([FromBody] SearchRequestContractBase searchRequest)
        {
            List<string> wordList = new List<string>();
            List<long> projectIdList = new List<long>();
            foreach (var searchCriteria in searchRequest.ConditionConjunction)
            {
                switch (searchCriteria.Key)
                {
                    case CriteriaKey.Fulltext:
                        var worldListCriteria = searchCriteria as WordListCriteriaContract;
                        if (worldListCriteria == null)
                            continue;
                        foreach (var disjunction in worldListCriteria.Disjunctions)
                        {
                            foreach (var contain in disjunction.Contains)
                            {
                                wordList.Add(contain);
                            }
                        }
                        break;
                    case CriteriaKey.SnapshotResultRestriction:
                        var resultRestrictionCriteria = searchCriteria as SnapshotResultRestrictionCriteriaContract;
                        if (resultRestrictionCriteria == null)
                            continue;

                        throw new NotImplementedException("Update search from projectId to snapshotId");
                        projectIdList.AddRange(resultRestrictionCriteria.SnapshotIds);
                        break;
                }
            }
            return m_searchManager.SearchByCriteria(wordList, projectIdList);
        }
    }
}
