using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.ProjectImport;
using Vokabular.ProjectImport.Model;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    [Authorize(PermissionNames.ManageBibliographyImport)]
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
       
        [HttpGet("importStatus")]
        public IList<RepositoryImportProgressInfo> GetActualProgress()
        {
            return m_mainImportManager.ActualProgress.Select(x => x.Value).ToList();
        }
    }
}