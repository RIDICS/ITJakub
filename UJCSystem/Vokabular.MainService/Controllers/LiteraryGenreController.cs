using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class LiteraryGenreController : BaseController
    {
        private readonly CatalogValueManager m_catalogValueManager;

        public LiteraryGenreController(CatalogValueManager catalogValueManager)
        {
            m_catalogValueManager = catalogValueManager;
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPost("")]
        public int CreateLiteraryGenre([FromBody] LiteraryGenreContract data)
        {
            var resultId = m_catalogValueManager.CreateLiteraryGenre(data.Name);
            return resultId;
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPut("{literaryGenreId}")]
        public IActionResult UpdateLiteraryGenre(int literaryGenreId, [FromBody] LiteraryGenreContract data)
        {
            try
            {
                m_catalogValueManager.UpdateLiteraryGenre(literaryGenreId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpDelete("{literaryGenreId}")]
        public IActionResult DeleteLiteraryGenre(int literaryGenreId)
        {
            try
            {
                m_catalogValueManager.DeleteLiteraryGenre(literaryGenreId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{literaryGenreId}")]
        [ProducesResponseType(typeof(LiteraryGenreContract), StatusCodes.Status200OK)]
        public IActionResult GetLiteraryGenre(int literaryGenreId)
        {
            var result = m_catalogValueManager.GetLiteraryGenre(literaryGenreId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        public List<LiteraryGenreContract> GetLiteraryGenreList()
        {
            var result = m_catalogValueManager.GetLiteraryGenreList();
            return result;
        }
    }
}