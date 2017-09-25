using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Clients.Errors;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class AuthorController : Controller
    {
        private readonly PersonManager m_personManager;

        public AuthorController(PersonManager personManager)
        {
            m_personManager = personManager;
        }

        [HttpPost("")]
        public int CreateOriginalAuthor([FromBody] OriginalAuthorContract author)
        {
            return m_personManager.CreateOriginalAuthor(author);
        }

        [HttpGet("autocomplete")]
        public List<OriginalAuthorContract> GetAutocomplete([FromQuery] string query)
        {
            return m_personManager.GetAuthorAutocomplete(query);
        }

        [HttpGet("{authorId}")]
        [ProducesResponseType(typeof(OriginalAuthorContract), StatusCodes.Status200OK)]
        public IActionResult GetOriginalAuthor(int authorId)
        {
            var result = m_personManager.GetOriginalAuthor(authorId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{authorId}")]
        public IActionResult UpdateOriginalAuthor(int authorId, [FromBody] OriginalAuthorContract author)
        {
            try
            {
                m_personManager.UpdateOriginalAuthor(authorId, author);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpDelete("{authorId}")]
        public IActionResult DeleteOriginalAuthor(int authorId)
        {
            try
            {
                m_personManager.DeleteOriginalAuthor(authorId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }
    }
}