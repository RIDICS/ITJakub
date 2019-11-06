using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Core.Structures.Shared;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class UserGroupController : BaseController
    {
        private readonly RoleManager m_roleManager;
        private readonly PermissionManager m_permissionManager;

        public UserGroupController(RoleManager roleManager, PermissionManager permissionManager)
        {
            m_roleManager = roleManager;
            m_permissionManager = permissionManager;
        }

        [Authorize(PermissionNames.ListUsers)]
        [HttpGet("{groupId}/user")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<UserContract> GetUsersByGroup(int groupId, [FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByName)
        {
            var result = m_roleManager.GetUsersByGroup(groupId, start, count, filterByName);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }

        [Authorize]
        [HttpGet("{groupId}/detail")]
        public RoleDetailContract GetUserGroupDetail(int groupId)
        {
            var result = m_roleManager.GetUserGroupDetail(groupId);
            return result;
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpGet("{groupId}/book/{bookId}/permission")]
        public PermissionDataContract GetPermissionsForGroupAndBook(int groupId, long bookId)
        {
            var result = m_permissionManager.GetPermissionsForGroupAndBook(groupId, bookId);
            return result;
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpPut("{groupId}/book/{bookId}/permission")]
        public void UpdateOrAddBooksToGroup(int groupId, long bookId, [FromBody] PermissionDataContract data)
        {
            m_permissionManager.UpdateOrAddBooksToGroup(groupId, new List<long> { bookId }, data);
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpDelete("{groupId}/book/{bookId}/permission")]
        public void RemoveBooksFromGroup(int groupId, long bookId)
        {
            m_permissionManager.RemoveBooksFromGroup(groupId, new List<long> { bookId });
        }
    }
}