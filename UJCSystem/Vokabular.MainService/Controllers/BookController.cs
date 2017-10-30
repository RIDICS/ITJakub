using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly BookManager m_bookManager;

        public BookController(BookManager bookManager)
        {
            m_bookManager = bookManager;
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

        [HttpPost("search")]
        public List<SearchResultContract> SearchBook([FromBody] SearchRequestContract request)
            // TODO possible switch SearchResultContract to BookContract
        {
            var result = m_bookManager.SearchByCriteria(request);
            return result;
        }

        [HttpPost("search-count")]
        public long SearchBookResultCount([FromBody] SearchRequestContract request)
        {
            var result = m_bookManager.SearchByCriteriaCount(request);
            return result;
        }

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
        public List<ChapterContract> GetBookChapterList(long projectId)
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

        [HttpGet("page/{pageId}/text")]
        public string GetPageText(long pageId, [FromQuery] TextFormatEnumContract? format)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_bookManager.GetPageText(pageId, formatValue);
            return result;
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
            Response.ContentLength = result.FileSize;
            return File(result.Stream, result.MimeType, result.FileName);
        }

        [HttpGet("audio/{audioId}/data")]
        public IActionResult GetAudio(long audioId)
        {
            var result = m_bookManager.GetAudio(audioId);
            Response.ContentLength = result.FileSize;
            return File(result.Stream, result.MimeType, result.FileName);
        }

        [HttpGet("headword/{headwordId}/text")]
        public string GetHeadwordText(long headwordId, [FromQuery] TextFormatEnumContract? format)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_bookManager.GetHeadwordText(headwordId, formatValue);
            return result;
        }
    }
}