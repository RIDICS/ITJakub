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
    public class FilteringExpressionSetController : BaseController
    {
        private readonly FilteringExpressionSetManager m_filteringExpressionSetManager;

        public FilteringExpressionSetController(FilteringExpressionSetManager filteringExpressionSetManager)
        {
            m_filteringExpressionSetManager = filteringExpressionSetManager;
        }

        [HttpPost("")]
        public int CreateFilteringExpressionSet([FromBody] FilteringExpressionSetDetailContract data)
        {
            var resultId = m_filteringExpressionSetManager.CreateFilteringExpressionSet(data);
            return resultId;
        }

        [HttpPut("{filteringExpressionSetId}")]
        public IActionResult UpdateExternalRepository(int filteringExpressionSetId, [FromBody] FilteringExpressionSetDetailContract data)
        {
            try
            {
                m_filteringExpressionSetManager.UpdateFilteringExpressionSet(filteringExpressionSetId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpDelete("{filteringExpressionSetId}")]
        public IActionResult DeleteExternalRepository(int filteringExpressionSetId)
        {
            try
            {
                m_filteringExpressionSetManager.DeleteFilteringExpressionSet(filteringExpressionSetId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{filteringExpressionSetId}")]
        [ProducesResponseType(typeof(FilteringExpressionSetDetailContract), StatusCodes.Status200OK)]
        public IActionResult GetFilteringExpressionSet(int filteringExpressionSetId)
        {
            var result = m_filteringExpressionSetManager.GetFilteringExpressionSet(filteringExpressionSetId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<FilteringExpressionSetContract> GetExternalRepositoryList([FromQuery] int? start, [FromQuery] int? count)
        {
            var result = m_filteringExpressionSetManager.GetFilteringExpressionSetList(start, count);
            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }
    }
}
