using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class AuthorController : BaseController
    {
        private readonly PersonManager m_personManager;
        private readonly ProjectManager m_projectManager;

        public AuthorController(PersonManager personManager, ProjectManager projectManager)
        {
            m_personManager = personManager;
            m_projectManager = projectManager;
        }

        [Authorize]
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

        [Authorize(VokabularPermissionNames.ManageCodeList)]
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

        [Authorize(VokabularPermissionNames.ManageCodeList)]
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
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<ProjectDetailContract> GetProjectsByAuthor(int authorId, [FromQuery] int? start, [FromQuery] int? count)
        {
            var result = m_projectManager.GetProjectsByAuthor(authorId, start, count);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }
    }
}