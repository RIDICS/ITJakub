using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search.Request;

namespace Vokabular.FulltextService.Controllers
{
    [Obsolete("This function has never worked as intended. It was replaced by BookPagedCorpus.")]
    [Route("api/[controller]")]
    public class CorpusController : ApiControllerBase
    {
        private readonly UnfinishedSearchManager m_searchManager;

        public CorpusController(UnfinishedSearchManager searchManager)
        {
            m_searchManager = searchManager;
        }

        /// <summary>
        /// Search in corpus
        /// </summary>
        /// <remarks>
        /// Search in corpus. Supported search criteria (key property - data type):
        /// - SnapshotResultRestriction - SnapshotResultRestrictionCriteriaContract
        /// - Fulltext - WordListCriteriaContract
        /// - Heading - WordListCriteriaContract
        /// - Sentence - WordListCriteriaContract
        /// - Headword - WordListCriteriaContract
        /// - HeadwordDescription - WordListCriteriaContract
        /// - TokenDistance - TokenDistanceListCriteriaContract
        /// - HeadwordDescriptionTokenDistance - TokenDistanceListCriteriaContract
        /// </remarks>
        /// <param name="searchRequest">
        /// Request contains list of search criteria with different data types described in method description
        /// </param>
        /// <returns></returns>
        [HttpPost("search")]
        public ActionResult<List<CorpusSearchResultContract>> SearchCorpus([FromBody] CorpusSearchRequestContract searchRequest)
        {
            if (ContainsAnyUnsupportedCriteria(searchRequest))
            {
                return BadRequest("Request contains unsupported criteria");
            }

            var result = m_searchManager.SearchCorpusByCriteria(searchRequest);
            return result;
        }

        /// <summary>
        /// Search in corpus, return count
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        [HttpPost("search-count")]
        public ActionResult<long> SearchCorpusResultCount([FromBody] CorpusSearchRequestContract searchRequest)
        {
            if (ContainsAnyUnsupportedCriteria(searchRequest))
            {
                return BadRequest("Request contains unsupported criteria");
            }

            var result = m_searchManager.SearchCorpusByCriteriaCount(searchRequest);
            return result.Count;
        }
    }
}