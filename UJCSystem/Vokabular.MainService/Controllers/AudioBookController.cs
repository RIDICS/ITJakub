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
    public class AudioBookController : Controller
    {
        private readonly BookManager m_bookManager;
        private readonly BookSearchManager m_bookSearchManager;

        public AudioBookController(BookManager bookManager, BookSearchManager bookSearchManager)
        {
            m_bookManager = bookManager;
            m_bookSearchManager = bookSearchManager;
        }
        
        /// <summary>
        /// Search audio books
        /// </summary>
        /// <remarks>
        /// Search audio book. Supported search criteria (key property - data type):
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
        [ProducesResponseType(typeof(List<AudioBookSearchResultContract>), StatusCodes.Status200OK)]
        public IActionResult SearchBook([FromBody] SearchRequestContract request)
        {
            try
            {
                var result = m_bookSearchManager.SearchAudioByCriteria(request);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Search audio books, return count
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("search-count")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult SearchBookResultCount([FromBody] SearchRequestContract request)
        {
            try
            {
                var result = m_bookSearchManager.SearchByCriteriaCount(request);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Get audio book detail with tracks and recordings
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("{projectId}")]
        public AudioBookSearchResultContract GetBookDetail(long projectId)
        {
            var result = m_bookManager.GetAudioBookDetail(projectId);
            return result;
        }
    }
}