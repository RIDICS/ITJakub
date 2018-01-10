using Microsoft.AspNetCore.Mvc;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.RequestContracts;
using Vokabular.Shared.DataContracts.Search.ResultContracts;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class CorpusController : Controller
    {
        private readonly SearchManager m_searchManager;

        public CorpusController(SearchManager searchManager)
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
        public CorpusSearchResultDataList SearchCorpus([FromBody] CorpusSearchRequestContract searchRequest)
        {
            var result = m_searchManager.SearchCorpusByCriteria(searchRequest);
            return result;
        }

        /// <summary>
        /// Search in corpus, return count
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        [HttpPost("search-count")]
        public long SearchCorpusResultCount([FromBody] CorpusSearchRequestContract searchRequest)
        {
            var result = m_searchManager.SearchCorpusByCriteriaCount(searchRequest);
            return result.Count;
        }
    }
}