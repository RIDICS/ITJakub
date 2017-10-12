using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
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

        [HttpGet("{projectId}")]
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

        [HttpGet("page/{pageId}/term")]
        public List<TermContract> GetPageTermList(long pageId)
        {
            var result = m_bookManager.GetPageTermList(pageId);
            return result;
        }

        [HttpHead("page/{pageId}/text")]
        public IActionResult HasPageText(long pageId)
        {
            var result = m_bookManager.HasBookPageText(pageId);
            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("page/{pageId}/text")]
        public string GetPageText(long pageId)
        {
            var result = m_bookManager.GetPageText(pageId);
            return result;
        }

        [HttpHead("page/{pageId}/image")]
        public IActionResult HasPageImage(long pageId)
        {
            var result = m_bookManager.HasBookPageImage(pageId);
            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("page/{pageId}/image")]
        public Stream GetPageImage(long pageId)
        {
            var result = m_bookManager.GetPageImage(pageId);
            return result;
        }
    }
}