using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.DataContracts;
using ITJakub.Web.Hub.Helpers;
using ITJakub.Web.Hub.Models.Requests.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Results;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(PermissionNames.ManagePermissions)]
    public class PermissionController : BaseController
    {
        private const int UserListPageSize = 5;

        public PermissionController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public ActionResult UserPermission(int start, int count, string query, bool partial)
        {
            using (var client = GetRestClient())
            {
                count = count == 0 ? UserListPageSize : count;
                var result = client.GetUserList(start, count, query);
                var model = CreateProjectListViewModel(result, start);

                if (partial)
                {
                    return PartialView("_UserList", model);
                }

                return View(model);
            }
        }

        public ActionResult GroupPermission()
        {
            return View();
        }

        public ActionResult GetTypeaheadUser(string query)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetUserAutocomplete(query);
                return Json(result);
            }
        }

        public ActionResult GetTypeaheadGroup(string query)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetRoleAutocomplete(query);
                return Json(result);
            }
        }

        public ActionResult GetUser(int userId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetUserDetail(userId);
                return Json(result);
            }
        }

        public ActionResult GetGroup(int groupId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetRoleDetail(groupId);
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult AddUserToGroup([FromBody] AddUserToGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.AddUserToRole(request.UserId, request.GroupId);
                return Json(new { });
            }
        }

        [HttpPost]
        public ActionResult CreateGroup([FromBody] CreateGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                var newUserGroupRequest = new RoleContract
                {
                    Name = request.GroupName,
                    Description = request.GroupDescription,
                };
                var groupId = client.CreateRole(newUserGroupRequest);
                var group = client.GetRoleDetail(groupId);
                return Json(group);
            }
        }

        [HttpPost]
        public ActionResult CreateGroupWithUser([FromBody] CreateGroupWithUserRequest request)
        {
            using (var client = GetRestClient())
            {
                var groupId = client.CreateRole(new RoleContract
                {
                    Name = request.GroupName,
                    Description = request.GroupDescription,
                });
                client.AddUserToRole(request.UserId, groupId);
                return Json(groupId);
            }
        }

        [HttpPost]
        public ActionResult RemoveUserFromGroup([FromBody] RemoveUserFromGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.RemoveUserFromRole(request.UserId, request.GroupId);
                return Json(new { });
            }
        }

        public ActionResult GetGroupsByUser(int userId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetRolesByUser(userId);
                return Json(result);
            }
        }

        public ActionResult GetRootCategories()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetBookTypeList();
                var convertedResult = result.Select(x => new CategoryOrBookTypeContract
                {
                    BookType = x.Type,
                    Description = BookTypeHelper.GetCategoryName(x.Type),
                });
                return Json(convertedResult);
            }
        }

        public ActionResult GetCategoryContent(int groupId, BookTypeEnumContract? bookType)
        {
            if (bookType == null)
            {
                return BadRequest("BookType parameter is required");
            }

            using (var client = GetRestClient())
            {
                var books = client.GetBooksForRole(groupId, bookType.Value);
                var result = new CategoryContentContract
                {
                    Categories = new List<CategoryContract>(), // Categories are currently not used after migration to new MainService
                    Books = books,
                };
                return Json(result);
            }
        }

        public ActionResult GetAllCategoryContent(BookTypeEnumContract? bookType)
        {
            if (bookType == null)
            {
                return BadRequest("BookType parameter is required");
            }

            using (var client = GetRestClient())
            {
                var books = client.GetAllBooksByType(bookType.Value);
                var result = new CategoryContentContract
                {
                    Categories = new List<CategoryContract>(), // Categories are currently not used after migration to new MainService
                    Books = books,
                };
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.DeleteRole(request.GroupId);
                return Json(new { });
            }
        }

        [HttpPost]
        public ActionResult AddBooksAndCategoriesToGroup([FromBody] AddBooksAndCategoriesToGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.AddBooksToRole(request.GroupId, request.BookIds);
                //client.AddBooksAndCategoriesToGroup(request.GroupId, request.BookIds, request.CategoryIds);
                return Json(new { });
            }
        }

        [HttpPost]
        public ActionResult RemoveBooksAndCategoriesFromGroup([FromBody] RemoveBooksAndCategoriesFromGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.RemoveBooksFromRole(request.GroupId, request.BookIds);
                //client.RemoveBooksAndCategoriesFromGroup(request.GroupId, request.BookIds, request.CategoryIds);
                return Json(new { });
            }
        }


        public ActionResult GetSpecialPermissionsForGroup(int groupId)
        {
            using (var client = GetRestClient())
            {
                var specialPermissions = client.GetSpecialPermissionsForRole(groupId);
                var result = specialPermissions.GroupBy(x => x.GetType().FullName).ToDictionary(x => x.Key, x => x.ToList());
                return Json(result);
            }
        }

        public ActionResult GetSpecialPermissions()
        {
            using (var client = GetRestClient())
            {
                var specialPermissions = client.GetSpecialPermissions();
                var result = specialPermissions.GroupBy(x => x.GetType().FullName).ToDictionary(x => x.Key, x => x.ToList());
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult AddSpecialPermissionsToGroup([FromBody] AddSpecialPermissionsToGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.AddSpecialPermissionsToRole(request.GroupId, request.SpecialPermissionIds);
                return Json(new { });
            }
        }

        [HttpPost]
        public ActionResult RemoveSpecialPermissionsFromGroup([FromBody] RemoveSpecialPermissionsFromGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.RemoveSpecialPermissionsFromRole(request.GroupId, request.SpecialPermissionIds);
                return Json(new { });
            }
        }

        private UserListViewModel CreateProjectListViewModel(PagedResultList<UserDetailContract> data, int start)
        {
            var listViewModel = Mapper.Map<List<UserDetailContract>>(data.List);
            return new UserListViewModel
            {
                TotalCount = data.TotalCount,
                List = listViewModel,
                PageSize = UserListPageSize,
                Start = start
            };
        }
    }
}