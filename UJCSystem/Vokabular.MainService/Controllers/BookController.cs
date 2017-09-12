using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

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

        [HttpGet("{bookType}")]
        [ProducesResponseType(typeof(List<BookWithCategoriesContract>), StatusCodes.Status200OK)]
        public IActionResult GetBooksByType(BookTypeEnumContract? bookType)
        {
            if (bookType == null)
                return NotFound();

            var result = m_bookManager.GetBooksByType(bookType.Value);
            return Ok(result);
        }

        [HttpPost("search")]
        public List<ProjectContract> SearchProject()
        {
            // TODO specify and test incomming data
            throw new NotImplementedException();
        }
    }
}