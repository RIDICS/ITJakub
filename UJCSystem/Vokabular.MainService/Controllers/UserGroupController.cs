using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class UserGroupController : BaseController
    {
        private readonly UserGroupManager m_userGroupManager;
        private readonly PermissionManager m_permissionManager;

        public UserGroupController(UserGroupManager userGroupManager, PermissionManager permissionManager)
        {
            m_userGroupManager = userGroupManager;
            m_permissionManager = permissionManager;
        }

        [HttpGet("{groupId}/user")]
        public List<UserContract> GetUsersByGroup(int groupId)
        {
            var result = m_userGroupManager.GetUsersByGroup(groupId);
            return result;
        }

        [HttpPost("")]
        public long CreateGroup([FromBody] UserGroupContract data)
        {
            var resultId = m_userGroupManager.CreateGroup(data.Name, data.Description);
            return resultId;
        }

        [HttpGet("{groupId}/detail")]
        public UserGroupDetailContract GetGroupDetail(int groupId)
        {
            var result = m_userGroupManager.GetGroupDetail(groupId);
            return result;
        }

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

        [HttpPost("{groupId}/permission/book")]
        public void AddBooksToGroup(int groupId, IList<long> bookIds/*, IList<int> categoryIds*/)
        {
            m_permissionManager.AddBooksAndCategoriesToGroup(groupId, bookIds);
        }

        [HttpDelete("{groupId}/permission/book")]
        public void RemoveBooksFromGroup(int groupId, IList<long> bookIds/*, IList<int> categoryIds*/)
        {
            m_permissionManager.RemoveBooksAndCategoriesFromGroup(groupId, bookIds);
        }

        [HttpDelete("{groupId}/user/{userId}")]
        public void RemoveUserFromGroup(int userId, int groupId)
        {
            m_userGroupManager.RemoveUserFromGroup(userId, groupId);
        }

        [HttpPost("{groupId}/user/{userId}")]
        public void AddUserToGroup(int userId, int groupId)
        {
            m_userGroupManager.AddUserToGroup(userId, groupId);
        }

        [HttpGet("{groupId}/book")] //TODO categoryId -> bookTypeId as filtering query parameter
        public object GetCategoryContentForGroup(int groupId, [FromQuery] int categoryId)
        //public CategoryContentContract GetCategoryContentForGroup(int groupId, int categoryId) // TODO use correct return type
        {
            throw new NotImplementedException();
        }

        //public CategoryContentContract GetAllCategoryContent(int categoryId) // TODO this method belongs to different controller
        //{
        //}

        [HttpGet("{groupId}/permission/special")]
        public List<SpecialPermissionContract> GetSpecialPermissionsForGroup(int groupId)
        {
            var result = m_permissionManager.GetSpecialPermissionsForGroup(groupId);
            return result;
        }

        [HttpPost("{groupId}/permission/special")]
        public void AddSpecialPermissionsToGroup(int groupId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            m_permissionManager.AddSpecialPermissionsToGroup(groupId, specialPermissionsIds.IdList);
        }

        [HttpDelete("{groupId}/permission/special")]
        public void RemoveSpecialPermissionsFromGroup(int groupId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            m_permissionManager.RemoveSpecialPermissionsFromGroup(groupId, specialPermissionsIds.IdList);
        }
    }
}