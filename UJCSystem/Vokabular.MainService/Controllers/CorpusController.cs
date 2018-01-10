using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search.RequestContracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class CorpusController : Controller
    {
        private readonly BookSearchManager m_bookSearchManager;

        public CorpusController(BookSearchManager bookSearchManager)
        {
            m_bookSearchManager = bookSearchManager;
        }

        /// <summary>
        /// Search in corpus
        /// </summary>
        /// <remarks>
        /// Search in corpus. Supported search criteria (key property - data type):
        /// - Author - WordListCriteriaContract
        /// - Title - WordListCriteriaContract
        /// - Editor - WordListCriteriaContract
        /// - Fulltext - WordListCriteriaContract
        /// - Heading - WordListCriteriaContract
        /// - Sentence - WordListCriteriaContract
        /// - Headword - WordListCriteriaContract
        /// - HeadwordDescription - WordListCriteriaContract
        /// - Term - WordListCriteriaContract
        /// - Dating - DatingListCriteriaContract
        /// - TokenDistance - TokenDistanceListCriteriaContract
        /// - HeadwordDescriptionTokenDistance - TokenDistanceListCriteriaContract
        /// - SelectedCategory - SelectedCategoryCriteriaContract
        /// </remarks>
        /// <param name="request">
        /// Request contains list of search criteria with different data types described in method description
        /// </param>
        /// <returns></returns>
        [HttpPost("search")]
        [ProducesResponseType(typeof(List<CorpusSearchResultContract>), StatusCodes.Status200OK)]
        public IActionResult SearchCorpus([FromBody] CorpusSearchRequestContract request)
        {
            try
            {
                var result = m_bookSearchManager.SearchCorpusByCriteria(request);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Search in corpus, return count
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("search-count")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult SearchCorpusResultCount([FromBody] CorpusSearchRequestContract request)
        {
            try
            {
                var result = m_bookSearchManager.SearchCorpusByCriteriaCount(request);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}