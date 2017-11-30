﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly BookManager m_bookManager;
        private readonly BookSearchManager m_bookSearchManager;

        public BookController(BookManager bookManager, BookSearchManager bookSearchManager)
        {
            m_bookManager = bookManager;
            m_bookSearchManager = bookSearchManager;
        }

        [HttpGet("type/{bookType}")]
        [ProducesResponseType(typeof(List<BookWithCategoriesContract>), StatusCodes.Status200OK)]
        public IActionResult GetBooksByType(BookTypeEnumContract? bookType)
        {
            if (bookType == null)
                return NotFound();

            var result = m_bookManager.GetBooksByType(bookType.Value);
            return Ok(result);
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
        /// <returns></returns>
        [HttpPost("search")]
        public List<SearchResultContract> SearchBook([FromBody] SearchRequestContract request)
            // TODO possible switch SearchResultContract to BookContract
        {
            var result = m_bookSearchManager.SearchByCriteria(request);
            return result;
        }

        /// <summary>
        /// Search books, return count
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("search-count")]
        public long SearchBookResultCount([FromBody] SearchRequestContract request)
        {
            var result = m_bookSearchManager.SearchByCriteriaCount(request);
            return result;
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
        public List<PageContract> SearchPage(long projectId, [FromBody] SearchPageRequestContract request)
        {
            var result = m_bookManager.SearchPage(projectId, request);
            return result;
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
            return hasAny ? (IActionResult)Ok() : NotFound();
        }

        [HttpHead("{projectId}/image")]
        public IActionResult HasBookAnyImage(long projectId)
        {
            var hasAny = m_bookManager.HasBookAnyImage(projectId);
            return hasAny ? (IActionResult)Ok() : NotFound();
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
            
            return Ok(result);
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
        public IActionResult GetPageTextFromSearch(long pageId, [FromQuery] TextFormatEnumContract? format, [FromBody] SearchPageRequestContract request)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_bookManager.GetPageText(pageId, formatValue, request);
            if (result == null)
                return NotFound();

            return Ok(result);
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

            return Ok(result);
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
        public IActionResult GetHeadwordTextFromSearch(long headwordId, [FromQuery] TextFormatEnumContract? format, [FromBody] SearchPageRequestContract request)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_bookManager.GetHeadwordText(headwordId, formatValue, request);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{projectId}/edition-note")]
        public IActionResult GetEditionNote(long projectId, [FromQuery] TextFormatEnumContract? format)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_bookManager.GetEditionNote(projectId, formatValue);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}