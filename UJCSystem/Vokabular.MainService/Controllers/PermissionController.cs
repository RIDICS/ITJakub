using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.Shared.Const;

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

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpGet("special")]
        public List<SpecialPermissionContract> GetSpecialPermissions()
        {
            var result = m_permissionManager.GetSpecialPermissions();
            return result;
        }

        [Authorize]
        [HttpPut("ensure")]
        public void EnsureAuthServiceHasRequiredPermissions()
        {
            m_permissionManager.EnsureAuthServiceHasRequiredPermissions();
        }
    }
}