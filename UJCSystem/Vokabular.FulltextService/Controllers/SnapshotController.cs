using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Request;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class SnapshotController : Controller
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<TextController>();

        private readonly SnapshotResourceManager m_snapshotResourceManager;
        
        private readonly SearchManager m_searchManager;

        public SnapshotController(SnapshotResourceManager snapshotResourceManager, TextResourceManager textResourceManager, SearchManager searchManager)
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
            var result = m_searchManager.SearchByCriteria(searchRequest);
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
            var result = m_searchManager.SearchByCriteriaCount(searchRequest);
            return result;
        }
    }
}