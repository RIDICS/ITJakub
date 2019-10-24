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
    public class RoleController : BaseController
    {
        private readonly RoleManager m_roleManager;
        private readonly PermissionManager m_permissionManager;

        public RoleController(RoleManager roleManager, PermissionManager permissionManager)
        {
            m_roleManager = roleManager;
            m_permissionManager = permissionManager;
        }

        [Authorize(PermissionNames.ListUsers)]
        [HttpGet("{roleId}/user")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<UserContract> GetUsersByRole(int roleId, [FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByName)
        {
            var result = m_roleManager.GetUsersByRole(roleId, start, count, filterByName);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }

        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpPost("")]
        public int CreateRole([FromBody] RoleContract data)
        {
            var resultId = m_roleManager.CreateRole(data.Name, data.Description);
            return resultId;
        }

        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpPut("{roleId}")]
        public IActionResult UpdateRole([FromBody] RoleContract data)
        {
            m_roleManager.UpdateRole(data);
            return Ok();
        }

        [Authorize]
        [HttpGet("{roleId}/detail")]
        public RoleDetailContract GetRoleDetail(int roleId)
        {
            var result = m_roleManager.GetRoleDetail(roleId);
            return result;
        }

        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpDelete("{roleId}")]
        public void DeleteRole(int roleId)
        {
            m_roleManager.DeleteRole(roleId);
        }

        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<RoleContract> GetRoleList([FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByName)
        {
            var result = m_roleManager.GetRoleList(start, count, filterByName);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpGet("{roleId}/book/{bookId}/permission")]
        public PermissionDataContract GetPermissionsForRoleAndBook(int roleId, long bookId)
        {
            var result = m_permissionManager.GetPermissionsForRoleAndBook(roleId, bookId);
            return result;
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpPut("{roleId}/book/{bookId}/permission")]
        public void UpdateOrAddBooksToRole(int roleId, long bookId, [FromBody] PermissionDataContract data)
        {
            m_permissionManager.UpdateOrAddBooksToRole(roleId, new List<long> {bookId}, data);
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpDelete("{roleId}/book/{bookId}/permission")]
        public void RemoveBooksFromRole(int roleId, long bookId)
        {
            m_permissionManager.RemoveBooksFromRole(roleId, new List<long> {bookId});
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpDelete("{roleId}/user/{userId}")]
        public void RemoveUserFromRole(int userId, int roleId)
        {
            m_roleManager.RemoveUserFromRole(userId, roleId);
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpPost("{roleId}/user/{userId}")]
        public void AddUserToRole(int userId, int roleId)
        {
            m_roleManager.AddUserToRole(userId, roleId);
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpPost("{roleId}/permission/special")]
        public void AddSpecialPermissionsToGroup(int roleId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            m_permissionManager.AddSpecialPermissionsToRole(roleId, specialPermissionsIds.IdList);
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpDelete("{roleId}/permission/special")]
        public void RemoveSpecialPermissionsFromGroup(int roleId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            m_permissionManager.RemoveSpecialPermissionsFromRole(roleId, specialPermissionsIds.IdList);
        }

        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpGet("autocomplete")]
        public List<RoleContract> GetAutocomplete([FromQuery] string query, [FromQuery] int? count)
        {
            var result = m_roleManager.GetRoleAutocomplete(query, count);
            return result;
        }
    }
}