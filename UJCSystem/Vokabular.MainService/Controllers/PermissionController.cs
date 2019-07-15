using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;

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

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<PermissionContract> GetPermissionList([FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByName)
        {
            var result = m_permissionManager.GetPermissions(start, count, filterByName);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }

        [Authorize]
        [HttpPut("ensure")]
        public void EnsureAuthServiceHasRequiredPermissions()
        {
            m_permissionManager.EnsureAuthServiceHasRequiredPermissions();
        }
    }
}