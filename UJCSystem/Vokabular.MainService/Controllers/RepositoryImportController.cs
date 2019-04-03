using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.ProjectImport;
using Vokabular.ProjectImport.Model;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class RepositoryImportController : BaseController
    {
        private readonly MainImportManager m_mainImportManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly AuthorizationManager m_authorizationManager;

        public RepositoryImportController(MainImportManager mainImportManager, AuthenticationManager authenticationManager, AuthorizationManager authorizationManager)
        {
            m_mainImportManager = mainImportManager;
            m_authenticationManager = authenticationManager;
            m_authorizationManager = authorizationManager;
        }

        [HttpPost("")]
        public IActionResult StartImport([FromBody] IList<int> externalRepositoryIds)
        {
            try
            {
                m_authorizationManager.CheckUserCanManageRepositoryImport();
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
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            return m_mainImportManager.ActualProgress.Select(x => x.Value).ToList();
        }
    }
}