using Microsoft.AspNetCore.Mvc;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search.Request;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class SnapshotController : Controller
    {
        private readonly SnapshotResourceManager m_snapshotResourceManager;
        
        private readonly SearchManager m_searchManager;

        public SnapshotController(SnapshotResourceManager snapshotResourceManager, SearchManager searchManager)
        {
            m_snapshotResourceManager = snapshotResourceManager;
            m_searchManager = searchManager;
        }

        [HttpPost]
        public ResultContract CreateSnapshot([FromBody] SnapshotPageIdsResourceContract snapshotPageIdsResourceContract)
        {
            var result = m_snapshotResourceManager.CreateSnapshotResource(snapshotPageIdsResourceContract);
            return result;
        }

        /// <summary>
        /// Search books
        /// </summary>
        /// <remarks>
        /// Search book. Supported search criteria (key property - data type):
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
        public FulltextSearchResultContract SearchByCriteria([FromBody] SearchRequestContract searchRequest)
        {
            var result = m_searchManager.SearchProjectsByCriteria(searchRequest);
            return result;
        }

        /// <summary>
        /// Search books, return count
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        [HttpPost("search-count")]
        public FulltextSearchResultContract SearchByCriteriaCount([FromBody] SearchRequestContractBase searchRequest)
        {
            var result = m_searchManager.SearchProjectsByCriteriaCount(searchRequest);
            return result;
        }
    }
}