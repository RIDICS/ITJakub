using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.ProjectImport.Managers;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ExternalRepositoryController : BaseController
    {
        private readonly ExternalRepositoryManager m_externalRepositoryManager;

        public ExternalRepositoryController(ExternalRepositoryManager externalRepositoryManager)
        {
            m_externalRepositoryManager = externalRepositoryManager;
        }

        [HttpPost("")]
        public int CreateExternalRepository([FromBody] ExternalRepositoryDetailContract data)
        {
            var resultId = m_externalRepositoryManager.CreateExternalRepository(data);
            return resultId;
        }

        [HttpPut("{externalRepositoryId}")]
        public IActionResult UpdateExternalRepository(int externalRepositoryId, [FromBody] ExternalRepositoryDetailContract data)
        {
            try
            {
                m_externalRepositoryManager.UpdateExternalRepository(externalRepositoryId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpDelete("{externalRepositoryId}")]
        public IActionResult DeleteExternalRepository(int externalRepositoryId)
        {
            try
            {
                m_externalRepositoryManager.DeleteExternalRepository(externalRepositoryId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{externalRepositoryId}")]
        [ProducesResponseType(typeof(ExternalRepositoryDetailContract), StatusCodes.Status200OK)]
        public IActionResult GetExternalRepository(int externalRepositoryId)
        {
            var result = m_externalRepositoryManager.GetExternalRepository(externalRepositoryId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<ExternalRepositoryDetailContract> GetExternalRepositoryList([FromQuery] int? start, [FromQuery] int? count)
        {
            var result = m_externalRepositoryManager.GetExternalRepositoryList(start, count);
            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }
    }
}
