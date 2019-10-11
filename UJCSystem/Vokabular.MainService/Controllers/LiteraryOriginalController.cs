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
    public class LiteraryOriginalController : BaseController
    {
        private readonly CatalogValueManager m_catalogValueManager;

        public LiteraryOriginalController(CatalogValueManager catalogValueManager)
        {
            m_catalogValueManager = catalogValueManager;
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPost("")]
        public int CreateLiteraryOriginal([FromBody] LiteraryOriginalContract literaryOriginal)
        {
            var resultId = m_catalogValueManager.CreateLiteraryOriginal(literaryOriginal.Name);
            return resultId;
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPut("{literaryOriginalId}")]
        public IActionResult UpdateLiteraryOriginal(int literaryOriginalId, [FromBody] LiteraryOriginalContract data)
        {
            try
            {
                m_catalogValueManager.UpdateLiteraryOriginal(literaryOriginalId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpDelete("{literaryOriginalId}")]
        public IActionResult DeleteLiteraryOriginal(int literaryOriginalId)
        {
            try
            {
                m_catalogValueManager.DeleteLiteraryOriginal(literaryOriginalId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{literaryOriginalId}")]
        [ProducesResponseType(typeof(LiteraryOriginalContract), StatusCodes.Status200OK)]
        public IActionResult GetLiteraryOriginal(int literaryOriginalId)
        {
            var result = m_catalogValueManager.GetLiteraryOriginal(literaryOriginalId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        public List<LiteraryOriginalContract> GetLiteraryOriginalList()
        {
            var result = m_catalogValueManager.GetLiteraryOriginalList();
            return result;
        }
    }
}