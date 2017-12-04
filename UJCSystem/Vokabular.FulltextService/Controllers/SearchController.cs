using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.ResultContracts;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<SearchController>();

        private readonly SearchManager m_searchManager;
        private readonly ITextConverter m_textConverter;

        public SearchController(SearchManager searchManager, ITextConverter textConverter)
        {
            m_searchManager = searchManager;
            m_textConverter = textConverter;
        }

        /// <summary>
        /// Search books, return count
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        [HttpPost("snapshot/count")]
        public FulltextSearchResultContract SearchByCriteriaCount([FromBody] SearchRequestContractBase searchRequest)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest), "Search request is null");
            }
            return m_searchManager.SearchByCriteriaCount(searchRequest);
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
        [HttpPost("snapshot")]
        public FulltextSearchResultContract SearchByCriteria([FromBody] SearchRequestContractBase searchRequest)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest), "Search request is null");
            }
            return m_searchManager.SearchByCriteria(searchRequest);
        }

        /// <summary>
        /// Search in corpus, return count
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        [HttpPost("snapshot/corpus/count")]
        public FulltextSearchCorpusResultContract SearchCorpusByCriteriaCount([FromBody] SearchRequestContractBase searchRequest)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest), "Search request is null");
            }
            return m_searchManager.SearchCorpusByCriteriaCount(searchRequest);
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
        [HttpPost("snapshot/corpus")]
        public CorpusSearchResultDataList SearchCorpusByCriteria([FromBody] CorpusSearchRequestContract searchRequest)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest), "Search request is null");
            }
            return m_searchManager.SearchCorpusByCriteria(searchRequest);
        }

        [HttpPost("page/{resourceId}")]
        public TextResourceContract SearchPageByCriteria(string textResourceId, [FromQuery] TextFormatEnumContract formatValue, [FromBody] SearchPageRequestContract searchRequest)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest), "Search request is null");
            }

            var textResource = m_searchManager.SearchPageByCriteria(textResourceId, searchRequest);
            textResource.PageText = m_textConverter.Convert(textResource.PageText, formatValue);

            return textResource;
        }


    }
}
