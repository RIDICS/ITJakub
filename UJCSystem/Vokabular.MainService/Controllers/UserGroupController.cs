using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class UserGroupController : BaseController
    {
        [HttpGet("{groupId}/user")]
        public List<UserContract> GetUsersByGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("")]
        public long CreateGroup([FromBody] GroupContract data)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{groupId}/detail")]
        public GroupDetailContract GetGroupDetail(int groupId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{groupId}")]
        public void DeleteGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        //public void AddBooksAndCategoriesToGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        //{
        //    //TODO split two methods - for books and categories
        //}

        //public void RemoveBooksAndCategoriesFromGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        //{
        //    //TODO split two methods - for books and categories
        //}

        [HttpDelete("{groupId}/user/{userId}")]
        public void RemoveUserFromGroup(int userId, int groupId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{groupId}/user/{userId}")]
        public void AddUserToGroup(int userId, int groupId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        [HttpPost("{groupId}/permission/special")]
        public void AddSpecialPermissionsToGroup(int groupId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{groupId}/permission/special")]
        public void RemoveSpecialPermissionsFromGroup(int groupId, [FromBody] IntegerIdListContract specialPermissionsIds)
        {
            throw new NotImplementedException();
        }
    }
}