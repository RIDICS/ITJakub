using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.ProjectImport.Managers;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Controllers
{
    [Route("api/bibliography/filtering-expression-set")]
    [Authorize(VokabularPermissionNames.ManageBibliographyImport)]
    public class FilteringExpressionSetController : BaseController
    {
        private readonly FilteringExpressionSetManager m_filteringExpressionSetManager;
        private readonly AuthenticationManager m_authenticationManager;

        public FilteringExpressionSetController(FilteringExpressionSetManager filteringExpressionSetManager,
            AuthenticationManager authenticationManager)
        {
            m_filteringExpressionSetManager = filteringExpressionSetManager;
            m_authenticationManager = authenticationManager;
        }

        [HttpPost("")]
        public int CreateFilteringExpressionSet([FromBody] FilteringExpressionSetDetailContract data)
        {
            var resultId = m_filteringExpressionSetManager.CreateFilteringExpressionSet(data, m_authenticationManager.GetCurrentUserId());
            return resultId;
        }

        [HttpPut("{filteringExpressionSetId}")]
        public IActionResult UpdateFilteringExpressionSet(int filteringExpressionSetId,
            [FromBody] FilteringExpressionSetDetailContract data)
        {
            try
            {
                m_filteringExpressionSetManager.UpdateFilteringExpressionSet(filteringExpressionSetId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        [HttpDelete("{filteringExpressionSetId}")]
        public IActionResult DeleteFilteringExpressionSet(int filteringExpressionSetId)
        {
            try
            {
                m_filteringExpressionSetManager.DeleteFilteringExpressionSet(filteringExpressionSetId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
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
        public List<FilteringExpressionSetContract> GetFilteringExpressionSetList([FromQuery] int? start, [FromQuery] int? count)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var result = m_filteringExpressionSetManager.GetFilteringExpressionSetList(startValue, countValue);
            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpGet("all")]
        public IList<FilteringExpressionSetContract> GetAllFilteringExpressionSets()
        {
            var result = m_filteringExpressionSetManager.GetAllFilteringExpressionSets();
            return result;
        }
    }

    [Route("api/bibliography/bibliography-format")]
    [Authorize(VokabularPermissionNames.ManageBibliographyImport)]
    public class BibliographyFormatController : BaseController
    {
        private readonly FilteringExpressionSetManager m_filteringExpressionSetManager;

        public BibliographyFormatController(FilteringExpressionSetManager filteringExpressionSetManager)
        {
            m_filteringExpressionSetManager = filteringExpressionSetManager;
        }

        [HttpGet("")]
        public IList<BibliographicFormatContract> GetAllBibliographicFormats()
        {
            return m_filteringExpressionSetManager.GetAllBibliographicFormats();
        }
    }
}