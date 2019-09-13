using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class BookController : BaseController
    {
        private readonly BookManager m_bookManager;
        private readonly BookSearchManager m_bookSearchManager;
        private readonly BookHitSearchManager m_bookHitSearchManager;
        private readonly EditionNoteManager m_editionNoteManager;

        public BookController(BookManager bookManager, BookSearchManager bookSearchManager, BookHitSearchManager bookHitSearchManager,
            EditionNoteManager editionNoteManager)
        {
            m_bookManager = bookManager;
            m_bookSearchManager = bookSearchManager;
            m_bookHitSearchManager = bookHitSearchManager;
            m_editionNoteManager = editionNoteManager;
        }

        [HttpGet("type/{bookType}")]
        [ProducesResponseType(typeof(List<BookWithCategoriesContract>), StatusCodes.Status200OK)]
        public IActionResult GetBooksByType(BookTypeEnumContract? bookType)
        {
            if (bookType == null)
                return NotFound();

            var result = m_bookManager.GetBooksByTypeForUser(bookType.Value);
            return Ok(result);
        }

        [HttpGet("type/{bookType}/all")]
        [ProducesResponseType(typeof(List<BookContract>), StatusCodes.Status200OK)]
        public IActionResult GetAllBooksByType(BookTypeEnumContract? bookType)
        {
            if (bookType == null)
                return NotFound();

            var result = m_bookManager.GetAllBooksByType(bookType.Value);
            return Ok(result);
        }

        [HttpGet("type")]
        public List<BookTypeContract> GetBookTypeList()
        {
            var result = m_bookManager.GetBookTypeList();
            return result;
        }

        /// <summary>
        /// Search books
        /// </summary>
        /// <remarks>
        /// Search book. Supported search criteria (key property - data type):
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
        /// <param name="projectType">Target project database for searching</param>
        /// <returns></returns>
        [HttpPost("search")]
        [ProducesResponseType(typeof(List<SearchResultContract>), StatusCodes.Status200OK)]
        public IActionResult SearchBook([FromBody] AdvancedSearchRequestContract request, [FromQuery] ProjectTypeContract? projectType)
            // TODO possible switch SearchResultContract to BookContract
        {
            if (projectType == null)
            {
                return Error($"Required parameter {nameof(projectType)} is not specified");
            }

            try
            {
                var result = m_bookSearchManager.SearchByCriteria(request, projectType.Value);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        /// <summary>
        /// Search books, return count
        /// </summary>
        /// <param name="request"></param>
        /// <param name="projectType"></param>
        /// <returns></returns>
        [HttpPost("search-count")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult SearchBookResultCount([FromBody] AdvancedSearchRequestContract request,
            [FromQuery] ProjectTypeContract? projectType)
        {
            if (projectType == null)
            {
                return Error($"Required parameter {nameof(projectType)} is not specified");
            }

            try
            {
                var result = m_bookSearchManager.SearchByCriteriaCount(request, projectType.Value);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        /// <summary>
        /// Search pages in specified book
        /// </summary>
        /// <remarks>
        /// Search pages. Supported search criteria (key property - data type):
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
        /// <param name="projectId">Book identification</param>
        /// <param name="request">
        /// Request contains list of search criteria with different data types described in method description
        /// </param>
        /// <returns></returns>
        [HttpPost("{projectId}/page/search")]
        [ProducesResponseType(typeof(List<PageContract>), StatusCodes.Status200OK)]
        public IActionResult SearchPage(long projectId, [FromBody] SearchPageRequestContract request)
        {
            try
            {
                var result = m_bookHitSearchManager.SearchPage(projectId, request);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        [HttpPost("{projectId}/hit/search")]
        [ProducesResponseType(typeof(List<PageResultContextContract>), StatusCodes.Status200OK)]
        public IActionResult SearchHitsWithPageContext(long projectId, [FromBody] SearchHitsRequestContract request)
        {
            try
            {
                var result = m_bookHitSearchManager.SearchHitsWithPageContext(projectId, request);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        [HttpPost("{projectId}/hit/search-count")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult SearchHitsResultCount(long projectId, [FromBody] SearchHitsRequestContract request)
        {
            try
            {
                var resultCount = m_bookHitSearchManager.SearchHitsResultCount(projectId, request);
                return Ok(resultCount);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{projectId}")]
        public BookContract GetBookInfo(long projectId)
        {
            var result = m_bookManager.GetBookInfo(projectId);
            return result;
        }

        [HttpGet("{projectId}/detail")]
        public SearchResultDetailContract GetBookDetail(long projectId)
        {
            var result = m_bookManager.GetBookDetail(projectId);
            return result;
        }

        [HttpGet("{projectId}/page")]
        public List<PageContract> GetBookPageList(long projectId)
        {
            var result = m_bookManager.GetBookPageList(projectId);
            return result;
        }

        [HttpGet("{projectId}/chapter")]
        public List<ChapterHierarchyContract> GetBookChapterList(long projectId)
        {
            var result = m_bookManager.GetBookChapterList(projectId);
            return result;
        }

        [HttpHead("{projectId}/text")]
        public IActionResult HasBookAnyText(long projectId)
        {
            var hasAny = m_bookManager.HasBookAnyText(projectId);
            return hasAny ? (IActionResult) Ok() : NotFound();
        }

        [HttpHead("{projectId}/image")]
        public IActionResult HasBookAnyImage(long projectId)
        {
            var hasAny = m_bookManager.HasBookAnyImage(projectId);
            return hasAny ? (IActionResult) Ok() : NotFound();
        }

        [HttpGet("page/{pageId}/term")]
        public List<TermContract> GetPageTermList(long pageId)
        {
            var result = m_bookManager.GetPageTermList(pageId);
            return result;
        }

        [HttpHead("page/{pageId}/text")]
        public IActionResult HasPageText(long pageId)
        {
            var hasText = m_bookManager.HasBookPageText(pageId);
            return hasText ? (IActionResult) Ok() : NotFound();
        }

        /// <summary>
        /// Load selected page text
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="format"></param>
        /// <returns>Selected page text</returns>
        [HttpGet("page/{pageId}/text")]
        public IActionResult GetPageText(long pageId, [FromQuery] TextFormatEnumContract? format)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_bookManager.GetPageText(pageId, formatValue);
            if (result == null)
                return NotFound();

            return Content(result);
        }

        /// <summary>
        /// Load selected page text with search highlights
        /// </summary>
        /// <remarks>
        /// Load selected page text with search highlights. Supported search criteria (key property - data type):
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
        /// <param name="pageId"></param>
        /// <param name="format"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("page/{pageId}/text/search")]
        public IActionResult GetPageTextFromSearch(long pageId, [FromQuery] TextFormatEnumContract? format,
            [FromBody] SearchPageRequestContract request)
        {
            try
            {
                var formatValue = format ?? TextFormatEnumContract.Html;
                var result = m_bookManager.GetPageText(pageId, formatValue, request);
                if (result == null)
                    return NotFound();

                return Content(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        [HttpHead("page/{pageId}/image")]
        public IActionResult HasPageImage(long pageId)
        {
            var hasImage = m_bookManager.HasBookPageImage(pageId);
            return hasImage ? (IActionResult) Ok() : NotFound();
        }

        [HttpGet("page/{pageId}/image")]
        public IActionResult GetPageImage(long pageId)
        {
            var result = m_bookManager.GetPageImage(pageId);
            if (result == null)
                return NotFound();

            Response.ContentLength = result.FileSize;
            return File(result.Stream, result.MimeType, result.FileName);
        }

        [HttpGet("audio/{audioId}/data")]
        public IActionResult GetAudio(long audioId)
        {
            var result = m_bookManager.GetAudio(audioId);
            if (result == null)
                return NotFound();

            Response.ContentLength = result.FileSize;
            return File(result.Stream, result.MimeType, result.FileName);
        }

        /// <summary>
        /// Load selected headword description text
        /// </summary>
        /// <param name="headwordId"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpGet("headword/{headwordId}/text")]
        public IActionResult GetHeadwordText(long headwordId, [FromQuery] TextFormatEnumContract? format)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_bookManager.GetHeadwordText(headwordId, formatValue);
            if (result == null)
                return NotFound();

            return Content(result);
        }

        /// <summary>
        /// Load selected headword description text with search highlights
        /// </summary>
        /// <remarks>
        /// Load selected headword description text with search highlights. Supported search criteria (key property - data type):
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
        /// <param name="headwordId"></param>
        /// <param name="format"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("headword/{headwordId}/text/search")]
        public IActionResult GetHeadwordTextFromSearch(long headwordId, [FromQuery] TextFormatEnumContract? format,
            [FromBody] SearchPageRequestContract request)
        {
            try
            {
                var formatValue = format ?? TextFormatEnumContract.Html;
                var result = m_bookManager.GetHeadwordText(headwordId, formatValue, request);
                if (result == null)
                    return NotFound();

                return Content(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{projectId}/edition-note/text")]
        public IActionResult GetEditionNoteText(long projectId, [FromQuery] TextFormatEnumContract? format)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_editionNoteManager.GetEditionNote(projectId, formatValue).Text;
            if (result == null)
                return NotFound();

            return Content(result);
        }

        [HttpGet("{projectId}/edition-note")]
        public EditionNoteContract GetEditionNote(long projectId, [FromQuery] TextFormatEnumContract? format)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_editionNoteManager.GetEditionNote(projectId, formatValue);
            return result;
        }

        [HttpGet("info")]
        [ProducesResponseType(typeof(BookContract), StatusCodes.Status200OK)]
        public IActionResult GetBookByExternalId([FromQuery] string externalId)
        {
            if (string.IsNullOrEmpty(externalId))
                return BadRequest("Required ExternalId parameter is null");

            var result = m_bookManager.GetBookInfoByExternalId(externalId);
            return result != null ? (IActionResult) Ok(result) : NotFound();
        }
    }
}