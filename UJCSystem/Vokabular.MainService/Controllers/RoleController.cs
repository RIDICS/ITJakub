using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class RoleController : BaseController
    {
        private readonly RoleManager m_roleManager;
        private readonly PermissionManager m_permissionManager;
        private readonly BookManager m_bookManager;

        public RoleController(RoleManager roleManager, PermissionManager permissionManager, BookManager bookManager)
        {
            m_roleManager = roleManager;
            m_permissionManager = permissionManager;
            m_bookManager = bookManager;
        }

        [HttpGet("{roleId}/user")]
        public List<UserContract> GetUsersByRole(int roleId)
        {
            var result = m_roleManager.GetUsersByRole(roleId);
            return result;
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpPost("")]
        public int CreateRole([FromBody] UserGroupContract data)
        {
            var resultId = m_roleManager.CreateRole(data.Name, data.Description);
            return resultId;
        }

        [HttpGet("{roleId}/detail")]
        public UserGroupContract GetRoleDetail(int roleId)
        {
            var result = m_roleManager.GetRoleDetail(roleId);
            return result;
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpDelete("{roleId}")]
        public void DeleteRole(int roleId)
        {
            m_roleManager.DeleteRole(roleId);
        }

        //public void AddBooksAndCategoriesToGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        //{
        //    //TODO split two methods - for books and categories
        //}

        //public void RemoveBooksAndCategoriesFromGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        //{
        //    //TODO split two methods - for books and categories
        //}

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpPost("{roleId}/permission/book")]
        public void AddBooksToGroup(int roleId, [FromBody] AddBookToUserGroupRequestContract request)
        {
            m_permissionManager.AddBooksAndCategoriesToGroup(roleId, request.BookIdList);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpDelete("{roleId}/permission/book")]
        public void RemoveBooksFromGroup(int roleId, [FromBody] AddBookToUserGroupRequestContract request)
        {
            m_permissionManager.RemoveBooksAndCategoriesFromGroup(roleId, request.BookIdList);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpDelete("{roleId}/user/{userId}")]
        public void RemoveUserFromRole(int userId, int roleId)
        {
            m_roleManager.RemoveUserFromRole(userId, roleId);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpPost("{roleId}/user/{userId}")]
        public void AddUserToRole(int userId, int roleId)
        {
            m_roleManager.AddUserToRole(userId, roleId);
        }

        [HttpGet("{roleId}/book")] //TODO categoryId -> bookTypeId as filtering query parameter
        [ProducesResponseType(typeof(List<BookContract>), StatusCodes.Status200OK)]
        public IActionResult GetBooksForRole(int roleId, [FromQuery] BookTypeEnumContract? filterByBookType)
        //public CategoryContentContract GetCategoryContentForGroup(int groupId, int categoryId) // TODO use correct return type
        {
            if (filterByBookType == null)
                return BadRequest();

            var result = m_bookManager.GetBooksForRole(roleId, filterByBookType.Value);
            return Ok(result);
        }

        //public CategoryContentContract GetAllCategoryContent(int categoryId) // TODO this method belongs to different controller
        //{
        //}

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpGet("{roleId}/permission/special")]
        public List<SpecialPermissionContract> GetSpecialPermissionsForRole(int roleId)
        {
            var result = m_permissionManager.GetSpecialPermissionsForRole(roleId);
            return result;
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpPost("{roleId}/permission/special")]
        public void AddSpecialPermissionsToGroup(int roleId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            m_permissionManager.AddSpecialPermissionsToRole(roleId, specialPermissionsIds.IdList);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpDelete("{roleId}/permission/special")]
        public void RemoveSpecialPermissionsFromGroup(int roleId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            m_permissionManager.RemoveSpecialPermissionsFromRole(roleId, specialPermissionsIds.IdList);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpGet("autocomplete")]
        public List<UserGroupContract> GetAutocomplete([FromQuery] string query, [FromQuery] int? count)
        {
            var result = m_roleManager.GetRoleAutocomplete(query, count);
            return result;
        }
    }
}