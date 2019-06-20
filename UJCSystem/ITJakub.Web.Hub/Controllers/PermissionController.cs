using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Constants;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.DataContracts;
using ITJakub.Web.Hub.Helpers;
using ITJakub.Web.Hub.Models;
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
        private const int UserListPageSize = 10;
        private const int GroupListPageSize = 10;
        private const int PermissionListPageSize = 10;

        public PermissionController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public ActionResult UserPermission(string search, int start, int count = UserListPageSize, ViewType viewType = ViewType.Full)
        {
            using (var client = GetRestClient())
            {
                search = search ?? string.Empty;
                var result = client.GetUserList(start, count, search);
                var model = CreateListViewModel<UserDetailViewModel, UserDetailContract>(result, start, UserListPageSize);

                ViewData[PermissionConstants.SearchUser] = search;

                switch (viewType)
                {
                    case ViewType.Partial:
                        return PartialView("_UserList", model);
                    case ViewType.Widget:
                        return PartialView("Widget/_UserListWidget", model);
                    case ViewType.Full:
                        return View(model);
                    default:
                        return View(model);
                }
            }
        }

        public ActionResult GroupPermission(string search, int start, int count = GroupListPageSize, ViewType viewType = ViewType.Full)
        {
            using (var client = GetRestClient())
            {
                search = search ?? string.Empty;
                var result = client.GetRoleList(start, count, search);
                var model = new ListViewModel<RoleContract>
                {
                    TotalCount = result.TotalCount,
                    List = result.List,
                    PageSize = GroupListPageSize,
                    Start = start
                };

                ViewData[PermissionConstants.SearchUser] = search;
                switch (viewType)
                {
                    case ViewType.Partial:
                        return PartialView("_GroupList", model);
                    case ViewType.Widget:
                        return PartialView("Widget/_GroupListWidget", model);
                    case ViewType.Full:
                        return View(model);
                    default:
                        return View(model);
                }
            }
        }

        public ActionResult GroupPermissionList(int roleId, string search, int start, int count = PermissionListPageSize, ViewType viewType = ViewType.Full)
        {
            using (var client = GetRestClient())
            {
                search = search ?? string.Empty;
                var roleContract = client.GetRoleDetail(roleId);
                var pagedPermissionsResult = client.GetPermissions(start,count, search);

                foreach (var permission in pagedPermissionsResult.List)
                {
                    permission.Selected = roleContract.Permissions.Any(x => x.Id == permission.Id);
                }

                var model = new ListViewModel<PermissionContract>
                {
                    TotalCount = pagedPermissionsResult.TotalCount,
                    List = pagedPermissionsResult.List,
                    PageSize = GroupListPageSize,
                    Start = start
                };

                ViewData[PermissionConstants.SearchPermission] = search;
          
                return PartialView("Widget/_PermissionListWidget", model);
            }
        }

        public ActionResult UsersByRole(int roleId, string search, int start, int count = GroupListPageSize)
        {
            using (var client = GetRestClient())
            {
                search = search ?? string.Empty;
                ViewData[PermissionConstants.SearchUser] = search;
                var result = client.GetUsersByRole(roleId, start, count, search);
                var model = CreateListViewModel<UserDetailViewModel, UserContract>(result, start, UserListPageSize);
                return PartialView("Widget/_UserListWidget", model);
            }
        }

        public ActionResult EditUser(int userId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetUserDetail(userId);
                var model = Mapper.Map<UpdateAccountViewModel>(result);
                return View(model);
            }
        }

        public ActionResult EditGroup(int groupId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetRoleDetail(groupId);
                return View(result);
            }
        }

        public ActionResult EditUserGroups(int userId)
        {
            return View();
        }

        public ActionResult EditRolePermissions(bool partial, string search, int start, int count = GroupListPageSize)
        {
            using (var client = GetRestClient())
            {
                search = search ?? string.Empty;
                var result = client.GetRoleList(start, count, search);
                var model = new ListViewModel<RoleContract>
                {
                    TotalCount = result.TotalCount,
                    List = result.List,
                    PageSize = GroupListPageSize,
                    Start = start
                };

                ViewData[PermissionConstants.SearchUser] = search;
                if (partial)
                {
                    return PartialView("_GroupList", model);
                }

                return View(model);
            }
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

        private ListViewModel<T> CreateListViewModel<T, T2>(PagedResultList<T2> data, int start, int pageSize)
        {
            return new ListViewModel<T>
            {
                TotalCount = data.TotalCount,
                List = Mapper.Map<List<T>>(data.List),
                PageSize = pageSize,
                Start = start
            };
        }
    }
}