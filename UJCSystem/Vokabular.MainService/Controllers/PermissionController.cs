using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class PermissionController : BaseController
    {
        private readonly PermissionManager m_permissionManager;

        public PermissionController(PermissionManager permissionManager)
        {
            m_permissionManager = permissionManager;
        }

        [HttpGet("special")]
        public List<SpecialPermissionContract> GetSpecialPermissions()
        {
            var result = m_permissionManager.GetSpecialPermissions();
            return result;
        }
    }
}