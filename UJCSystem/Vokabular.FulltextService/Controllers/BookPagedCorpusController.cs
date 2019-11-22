using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Request;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class BookPagedCorpusController : ApiControllerBase
    {
        private readonly SearchManager m_searchManager;

        public BookPagedCorpusController(SearchManager searchManager)
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
        public ActionResult<CorpusSearchSnapshotsResultContract> SearchCorpusSnapshots([FromBody] BookPagedCorpusSearchRequestContract searchRequest)
        {
            if (ContainsAnyUnsupportedCriteria(searchRequest))
            {
                return BadRequest("Request contains unsupported criteria");
            }

            var result = m_searchManager.SearchCorpusSnapshotsByCriteria(searchRequest);
            return result;
        }

        [HttpPost("snapshot/{snapshotId}/search")]
        public ActionResult<List<CorpusSearchResultContract>> SearchCorpusSnapshot(long snapshotId, [FromBody] BookPagedCorpusSearchInSnapshotRequestContract searchRequest)
        {
            if (ContainsAnyUnsupportedCriteria(searchRequest))
            {
                return BadRequest("Request contains unsupported criteria");
            }

            var result = m_searchManager.SearchCorpusSnapshotByCriteria(snapshotId, searchRequest);
            return result;
        }

        [HttpPost("search-count")]
        public ActionResult<long> SearchCorpusSnapshotsCount([FromBody] SearchRequestContractBase searchRequest)
        {
            if (ContainsAnyUnsupportedCriteria(searchRequest))
            {
                return BadRequest("Request contains unsupported criteria");
            }

            var result = m_searchManager.SearchCorpusSnapshotsByCriteriaCount(searchRequest);
            return result.Result;
        }
    }
}