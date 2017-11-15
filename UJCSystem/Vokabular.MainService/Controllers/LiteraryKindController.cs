using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class LiteraryKindController : Controller
    {
        private CatalogValueManager m_catalogValueManager;

        public LiteraryKindController(CatalogValueManager catalogValueManager)
        {
            m_catalogValueManager = catalogValueManager;
        }

        [HttpPost("")]
        public int CreateLiteraryKind([FromBody] LiteraryKindContract literaryKind)
        {
            var resultId = m_catalogValueManager.CreateLiteraryKind(literaryKind.Name);
            return resultId;
        }

        [HttpPut("{literaryKindId}")]
        public IActionResult UpdateLiteraryKind(int literaryKindId, [FromBody] LiteraryKindContract data)
        {
            try
            {
                m_catalogValueManager.UpdateLiteraryKind(literaryKindId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpDelete("{literaryKindId}")]
        public IActionResult DeleteLiteraryKind(int literaryKindId)
        {
            try
            {
                m_catalogValueManager.DeleteLiteraryKind(literaryKindId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{literaryKindId}")]
        [ProducesResponseType(typeof(LiteraryKindContract), StatusCodes.Status200OK)]
        public IActionResult GetLiteraryKind(int literaryKindId)
        {
            var result = m_catalogValueManager.GetLiteraryKind(literaryKindId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            var result = m_catalogValueManager.GetLiteraryKindList();
            return result;
        }
    }
}