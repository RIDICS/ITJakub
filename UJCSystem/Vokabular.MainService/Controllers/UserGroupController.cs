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
    public class UserGroupController : BaseController
    {
        private readonly UserGroupManager m_userGroupManager;
        private readonly PermissionManager m_permissionManager;
        private readonly BookManager m_bookManager;

        public UserGroupController(UserGroupManager userGroupManager, PermissionManager permissionManager, BookManager bookManager)
        {
            m_userGroupManager = userGroupManager;
            m_permissionManager = permissionManager;
            m_bookManager = bookManager;
        }

        [HttpGet("{groupId}/user")]
        public List<UserContract> GetUsersByGroup(int groupId)
        {
            var result = m_userGroupManager.GetUsersByGroup(groupId);
            return result;
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpPost("")]
        public int CreateGroup([FromBody] UserGroupContract data)
        {
            var resultId = m_userGroupManager.CreateGroup(data.Name, data.Description);
            return resultId;
        }

        [HttpGet("{groupId}/detail")]
        public UserGroupContract GetGroupDetail(int groupId)
        {
            var result = m_userGroupManager.GetGroupDetail(groupId);
            return result;
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpDelete("{groupId}")]
        public void DeleteGroup(int groupId)
        {
            m_userGroupManager.DeleteGroup(groupId);
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
        [HttpPost("{groupId}/permission/book")]
        public void AddBooksToGroup(int groupId, [FromBody] AddBookToUserGroupRequestContract request)
        {
            m_permissionManager.AddBooksAndCategoriesToGroup(groupId, request.BookIdList);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpDelete("{groupId}/permission/book")]
        public void RemoveBooksFromGroup(int groupId, [FromBody] AddBookToUserGroupRequestContract request)
        {
            m_permissionManager.RemoveBooksAndCategoriesFromGroup(groupId, request.BookIdList);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpDelete("{groupId}/user/{userId}")]
        public void RemoveUserFromGroup(int userId, int groupId)
        {
            m_userGroupManager.RemoveUserFromGroup(userId, groupId);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpPost("{groupId}/user/{userId}")]
        public void AddUserToGroup(int userId, int groupId)
        {
            m_userGroupManager.AddUserToGroup(userId, groupId);
        }

        [HttpGet("{groupId}/book")] //TODO categoryId -> bookTypeId as filtering query parameter
        [ProducesResponseType(typeof(List<BookContract>), StatusCodes.Status200OK)]
        public IActionResult GetBooksForGroup(int groupId, [FromQuery] BookTypeEnumContract? filterByBookType)
        //public CategoryContentContract GetCategoryContentForGroup(int groupId, int categoryId) // TODO use correct return type
        {
            if (filterByBookType == null)
                return BadRequest();

            var result = m_bookManager.GetBooksForUserGroup(groupId, filterByBookType.Value);
            return Ok(result);
        }

        //public CategoryContentContract GetAllCategoryContent(int categoryId) // TODO this method belongs to different controller
        //{
        //}

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpGet("{groupId}/permission/special")]
        public List<SpecialPermissionContract> GetSpecialPermissionsForGroup(int groupId)
        {
            var result = m_permissionManager.GetSpecialPermissionsForGroup(groupId);
            return result;
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpPost("{groupId}/permission/special")]
        public void AddSpecialPermissionsToGroup(int groupId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            m_permissionManager.AddSpecialPermissionsToGroup(groupId, specialPermissionsIds.IdList);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpDelete("{groupId}/permission/special")]
        public void RemoveSpecialPermissionsFromGroup(int groupId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            m_permissionManager.RemoveSpecialPermissionsFromGroup(groupId, specialPermissionsIds.IdList);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpGet("autocomplete")]
        public List<UserGroupContract> GetAutocomplete([FromQuery] string query, [FromQuery] int? count)
        {
            var result = m_userGroupManager.GetUserGroupAutocomplete(query, count);
            return result;
        }
    }
}