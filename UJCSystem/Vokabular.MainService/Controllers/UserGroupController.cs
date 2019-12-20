using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Core.Structures.Shared;
using Vokabular.DataEntities.Database.Entities.Enums;
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
        private readonly AuthorizationManager m_authorizationManager;

        public UserGroupController(RoleManager roleManager, PermissionManager permissionManager, AuthorizationManager authorizationManager)
        {
            m_roleManager = roleManager;
            m_permissionManager = permissionManager;
            m_authorizationManager = authorizationManager;
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

        [HttpGet("{groupId}/book/{bookId}/permission")]
        public PermissionDataContract GetPermissionsForGroupAndBook(int groupId, long bookId)
        {
            m_authorizationManager.AuthorizeBookOrPermission(bookId, PermissionFlag.ReadProject, PermissionNames.AssignPermissionsToRoles);

            var result = m_permissionManager.GetPermissionsForGroupAndBook(groupId, bookId);
            return result;
        }

        [HttpPost("{groupId}/book/{bookId}/permission")]
        public void AddBookToGroup(int groupId, long bookId, [FromBody] PermissionDataContract data)
        {
            m_authorizationManager.AuthorizeBookOrPermission(bookId, PermissionFlag.AdminProject, PermissionNames.AssignPermissionsToRoles);

            m_permissionManager.AddBookToGroup(groupId, bookId, data);
        }

        [HttpPut("{groupId}/book/{bookId}/permission")]
        public void UpdateOrAddBookToGroup(int groupId, long bookId, [FromBody] PermissionDataContract data)
        {
            m_authorizationManager.AuthorizeBookOrPermission(bookId, PermissionFlag.AdminProject, PermissionNames.AssignPermissionsToRoles);

            m_permissionManager.UpdateOrAddBooksToGroup(groupId, new List<long> { bookId }, data);
        }

        [HttpDelete("{groupId}/book/{bookId}/permission")]
        public void RemoveBooksFromGroup(int groupId, long bookId)
        {
            m_authorizationManager.AuthorizeBookOrPermission(bookId, PermissionFlag.AdminProject, PermissionNames.AssignPermissionsToRoles);

            m_permissionManager.RemoveBooksFromGroup(groupId, new List<long> { bookId });
        }
    }
}