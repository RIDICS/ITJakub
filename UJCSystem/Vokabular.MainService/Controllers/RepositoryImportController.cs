using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.ExternalBibliography;
using Vokabular.ProjectImport;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Controllers
{
    [Route("api/bibliography/import")]
    [Authorize(VokabularPermissionNames.ManageBibliographyImport)]
    public class RepositoryImportController : BaseController
    {
        private readonly MainImportManager m_mainImportManager;
        private readonly AuthenticationManager m_authenticationManager;

        public RepositoryImportController(MainImportManager mainImportManager, AuthenticationManager authenticationManager)
        {
            m_mainImportManager = mainImportManager;
            m_authenticationManager = authenticationManager;
        }

        [HttpPost("")]
        public IActionResult StartImport([FromBody] IList<int> externalRepositoryIds)
        {
            try
            {
                m_mainImportManager.ImportFromResources(externalRepositoryIds, m_authenticationManager.GetCurrentUserId());
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }
       
        [HttpGet("status")]
        public IList<RepositoryImportProgressInfoContract> GetActualProgress()
        {
            return m_mainImportManager.GetActualProgressInfo();
        }
    }
}