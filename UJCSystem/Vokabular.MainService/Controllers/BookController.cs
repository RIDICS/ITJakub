using System.Collections.Generic;
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
    }
}