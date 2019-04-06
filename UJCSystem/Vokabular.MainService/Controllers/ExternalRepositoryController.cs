﻿using System.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
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

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ExternalRepositoryController : BaseController
    {
        private readonly ExternalRepositoryManager m_externalRepositoryManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly MainImportManager m_mainImportManager;

        public ExternalRepositoryController(ExternalRepositoryManager externalRepositoryManager,
            AuthenticationManager authenticationManager, AuthorizationManager authorizationManager, MainImportManager mainImportManager)
        {
            m_externalRepositoryManager = externalRepositoryManager;
            m_authenticationManager = authenticationManager;
            m_authorizationManager = authorizationManager;
            m_mainImportManager = mainImportManager;
        }

        [HttpPost("")]
        public int CreateExternalRepository([FromBody] ExternalRepositoryDetailContract data)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            var resultId = m_externalRepositoryManager.CreateExternalRepository(data, m_authenticationManager.GetCurrentUserId());
            return resultId;
        }

        [HttpPut("{externalRepositoryId}")]
        public IActionResult UpdateExternalRepository(int externalRepositoryId, [FromBody] ExternalRepositoryDetailContract data)
        {
            try
            {
                m_authorizationManager.CheckUserCanManageRepositoryImport();
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
                m_authorizationManager.CheckUserCanManageRepositoryImport();
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
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            var result = m_externalRepositoryManager.GetExternalRepository(externalRepositoryId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{externalRepositoryId}/statistics")]
        [ProducesResponseType(typeof(ExternalRepositoryStatisticsContract), StatusCodes.Status200OK)]
        public IActionResult GetExternalRepositoryStatistics(int externalRepositoryId)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            var result = m_externalRepositoryManager.GetExternalRepositoryStatistics(externalRepositoryId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<ExternalRepositoryDetailContract> GetExternalRepositoryList([FromQuery] int? start, [FromQuery] int? count)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();

            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var result = m_externalRepositoryManager.GetExternalRepositoryList(startValue, countValue);
            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpDelete("{externalRepositoryId}/importStatus")]
        public IActionResult CancelImportTask(int externalRepositoryId)
        {
            try
            {
                m_authorizationManager.CheckUserCanManageRepositoryImport();
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
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            return m_externalRepositoryManager.GetAllExternalRepositoryTypes();
        }

        #region OAI-PMH

        [HttpGet("oaiPmhRepositoryInfo/{url}")]
        public async Task<OaiPmhRepositoryInfoContract> GetOaiPmhRepositoryInfoAsync(string url)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            return await m_externalRepositoryManager.GetOaiPmhRepositoryInfo(HttpUtility.UrlDecode(url));
        }

        #endregion
    }
}