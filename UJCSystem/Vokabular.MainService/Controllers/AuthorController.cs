using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.Utils.Documentation;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class AuthorController : BaseController
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
        public List<OriginalAuthorContract> GetAutocomplete([FromQuery] string query, [FromQuery] BookTypeEnumContract? bookType, [FromQuery] int? count)
        {
            return m_personManager.GetAuthorAutocomplete(query, bookType, count);
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

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total records count")]
        public List<OriginalAuthorContract> GetOriginalAuthorList([FromQuery] int? start, [FromQuery] int? count)
        {
            var result = m_personManager.GetOriginalAuthorList(start, count);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }

        [HttpGet("{authorId}/project")]
        public ProjectDetailContract GetProjectByAuthor(int authorId)
        {
            throw new NotImplementedException();
        }
    }
}