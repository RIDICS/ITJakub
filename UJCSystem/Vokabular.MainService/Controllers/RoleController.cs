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
        
        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpPost("")]
        public int CreateRole([FromBody] RoleContract data)
        {
            var resultId = m_roleManager.CreateRole(data.Name, data.Description);
            return resultId;
        }

        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpPut("{groupId}")]
        public IActionResult UpdateRole(int groupId, [FromBody] RoleContract data)
        {
            data.Id = groupId;
            m_roleManager.UpdateRole(data);
            return Ok();
        }
        
        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpDelete("{groupId}")]
        public void DeleteRole(int groupId)
        {
            m_roleManager.DeleteRole(groupId);
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
        
        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpDelete("{groupId}/user/{userId}")]
        public void RemoveUserFromRole(int userId, int groupId)
        {
            m_roleManager.RemoveUserFromRole(userId, groupId);
        }

        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpPost("{groupId}/user/{userId}")]
        public void AddUserToRole(int userId, int groupId)
        {
            m_roleManager.AddUserToRole(userId, groupId);
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpPost("{groupId}/permission/special")]
        public void AddSpecialPermissionsToGroup(int groupId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            m_permissionManager.AddSpecialPermissionsToRole(groupId, specialPermissionsIds.IdList);
        }

        [Authorize(PermissionNames.AssignPermissionsToRoles)]
        [HttpDelete("{groupId}/permission/special")]
        public void RemoveSpecialPermissionsFromGroup(int groupId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            m_permissionManager.RemoveSpecialPermissionsFromRole(groupId, specialPermissionsIds.IdList);
        }

        //[Authorize(PermissionNames.ManageUserRoles)]
        [HttpGet("autocomplete")]
        public List<RoleContract> GetAutocomplete([FromQuery] string query, [FromQuery] int? count)
        {
            var result = m_roleManager.GetRoleAutocomplete(query, count);
            return result;
        }
    }
}