using System.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;
using Vokabular.ProjectImport;
using Vokabular.ProjectImport.Managers;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    [Authorize(PermissionNames.ManageBibliographyImport)]
    public class ExternalRepositoryController : BaseController
    {
        private readonly ExternalRepositoryManager m_externalRepositoryManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly MainImportManager m_mainImportManager;

        public ExternalRepositoryController(ExternalRepositoryManager externalRepositoryManager,
            AuthenticationManager authenticationManager, MainImportManager mainImportManager)
        {
            m_externalRepositoryManager = externalRepositoryManager;
            m_authenticationManager = authenticationManager;
            m_mainImportManager = mainImportManager;
        }

        [HttpPost("")]
        public int CreateExternalRepository([FromBody] ExternalRepositoryDetailContract data)
        {
            var resultId = m_externalRepositoryManager.CreateExternalRepository(data, m_authenticationManager.GetCurrentUserId());
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
                return StatusCode((int) exception.StatusCode, exception.Message);
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
                return StatusCode((int) exception.StatusCode, exception.Message);
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

        [HttpGet("{externalRepositoryId}/statistics")]
        [ProducesResponseType(typeof(ExternalRepositoryStatisticsContract), StatusCodes.Status200OK)]
        public IActionResult GetExternalRepositoryStatistics(int externalRepositoryId)
        {
            var result = m_externalRepositoryManager.GetExternalRepositoryStatistics(externalRepositoryId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<ExternalRepositoryContract> GetExternalRepositoryList([FromQuery] int? start, [FromQuery] int? count)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var result = m_externalRepositoryManager.GetExternalRepositoryList(startValue, countValue);
            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpGet("allExternalRepositories")]
        public IList<ExternalRepositoryContract> GetAllExternalRepositories()
        {
            var result = m_externalRepositoryManager.GetAllExternalRepositories();
            return result;
        }

        [HttpDelete("{externalRepositoryId}/importStatus")]
        public IActionResult CancelImportTask(int externalRepositoryId)
        {
            try
            {
                m_mainImportManager.CancelTask(externalRepositoryId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("allExternalRepositoryTypes")]
        public IList<ExternalRepositoryTypeContract> GetAllExternalRepositoryTypes()
        {
            return m_externalRepositoryManager.GetAllExternalRepositoryTypes();
        }

        #region OAI-PMH

        [HttpGet("oaiPmhRepositoryInfo")]
        public async Task<OaiPmhRepositoryInfoContract> GetOaiPmhRepositoryInfoAsync([FromQuery] string url)
        {
            return await m_externalRepositoryManager.GetOaiPmhRepositoryInfo(url);
        }

        #endregion
    }
}