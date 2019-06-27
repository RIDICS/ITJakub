using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
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
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<UserContract> GetUsersByRole(int roleId, [FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByName)
        {
            var result = m_roleManager.GetUsersByRole(roleId, start, count, filterByName);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpPost("")]
        public int CreateRole([FromBody] RoleContract data)
        {
            var resultId = m_roleManager.CreateRole(data.Name, data.Description);
            return resultId;
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpPut("{roleId}/edit")]
        public IActionResult UpdateRole([FromBody] RoleContract data)
        {
            m_roleManager.UpdateRole(data);
            return Ok();
        }

        [HttpGet("{roleId}/detail")]
        public RoleDetailContract GetRoleDetail(int roleId)
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

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<RoleContract> GetRoleList([FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByName)
        {
            var result = m_roleManager.GetRoleList(start, count, filterByName);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
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
        public void AddBooksToGroup(int roleId, [FromBody] AddBookToRoleRequestContract request)
        {
            m_permissionManager.AddBooksAndCategoriesToGroup(roleId, request.BookIdList);
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpDelete("{roleId}/permission/book")]
        public void RemoveBooksFromGroup(int roleId, [FromBody] AddBookToRoleRequestContract request)
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
        public List<RoleContract> GetAutocomplete([FromQuery] string query, [FromQuery] int? count)
        {
            var result = m_roleManager.GetRoleAutocomplete(query, count);
            return result;
        }
    }
}