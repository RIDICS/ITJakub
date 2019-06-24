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
        private const int UserListPageSize = 1;
        private const int RoleListPageSize = 3;
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
                var model = CreateListViewModel<UserDetailViewModel, UserDetailContract>(result, start, count);

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

        public ActionResult RolePermission(string search, int start, int count = RoleListPageSize, ViewType viewType = ViewType.Full)
        {
            using (var client = GetRestClient())
            {
                search = search ?? string.Empty;
                var result = client.GetRoleList(start, count, search);
                var model = new ListViewModel<RoleContract>
                {
                    TotalCount = result.TotalCount,
                    List = result.List,
                    PageSize = count,
                    Start = start
                };

                ViewData[PermissionConstants.SearchUser] = search;
                switch (viewType)
                {
                    case ViewType.Partial:
                        return PartialView("_RoleList", model);
                    case ViewType.Widget:
                        return PartialView("Widget/_RoleListWidget", model);
                    case ViewType.Full:
                        return View(model);
                    default:
                        return View(model);
                }
            }
        }

        public ActionResult RolePermissionList(int roleId, string search, int start, int count = PermissionListPageSize, ViewType viewType = ViewType.Full)
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
                    PageSize = count,
                    Start = start
                };

                ViewData[PermissionConstants.SearchPermission] = search;
          
                return PartialView("Widget/_PermissionListWidget", model);
            }
        }

        public ActionResult UsersByRole(int roleId, string search, int start, int count = UserListPageSize)
        {
            using (var client = GetRestClient())
            {
                search = search ?? string.Empty;
                ViewData[PermissionConstants.SearchUser] = search;
                var result = client.GetUsersByRole(roleId, start, count, search);
                var model = CreateListViewModel<UserDetailViewModel, UserContract>(result, start, count);
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

        public ActionResult EditRole(int roleId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetRoleDetail(roleId);
                return View(result);
            }
        }

        public ActionResult EditUserRoles(int userId)
        {
            return View();
        }

        public ActionResult EditRolePermissions(bool partial, string search, int start, int count = RoleListPageSize)
        {
            using (var client = GetRestClient())
            {
                search = search ?? string.Empty;
                var result = client.GetRoleList(start, count, search);
                var model = new ListViewModel<RoleContract>
                {
                    TotalCount = result.TotalCount,
                    List = result.List,
                    PageSize = count,
                    Start = start
                };

                ViewData[PermissionConstants.SearchRole] = search;
                if (partial)
                {
                    return PartialView("_RoleList", model);
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
        public ActionResult AddUserToGroup([FromBody] AddUserToRoleRequest request)
        {
            using (var client = GetRestClient())
            {
                client.AddUserToRole(request.UserId, request.RoleId);
                return Json(new { });
            }
        }

        [HttpPost]
        public ActionResult CreateGroup([FromBody] CreateRoleRequest request)
        {
            using (var client = GetRestClient())
            {
                var newUserGroupRequest = new RoleContract
                {
                    Name = request.RoleName,
                    Description = request.RoleDescription,
                };
                var groupId = client.CreateRole(newUserGroupRequest);
                var group = client.GetRoleDetail(groupId);
                return Json(group);
            }
        }

        [HttpPost]
        public ActionResult CreateGroupWithUser([FromBody] CreateRoleWithUserRequest request)
        {
            using (var client = GetRestClient())
            {
                var groupId = client.CreateRole(new RoleContract
                {
                    Name = request.RoleName,
                    Description = request.RoleDescription,
                });
                client.AddUserToRole(request.UserId, groupId);
                return Json(groupId);
            }
        }

        [HttpPost]
        public ActionResult RemoveUserFromRole([FromBody] RemoveUserFromRoleRequest request)
        {
            using (var client = GetRestClient())
            {
                client.RemoveUserFromRole(request.UserId, request.RoleId);
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
        public ActionResult DeleteRole([FromBody] DeleteRoleRequest request)
        {
            using (var client = GetRestClient())
            {
                client.DeleteRole(request.RoleId);
                return Json(new { });
            }
        }

        [HttpPost]
        public ActionResult AddBooksAndCategoriesToGroup([FromBody] AddBooksAndCategoriesToRoleRequest request)
        {
            using (var client = GetRestClient())
            {
                client.AddBooksToRole(request.RoleId, request.BookIds);
                //client.AddBooksAndCategoriesToGroup(request.GroupId, request.BookIds, request.CategoryIds);
                return Json(new { });
            }
        }

        [HttpPost]
        public ActionResult RemoveBooksAndCategoriesFromGroup([FromBody] RemoveBooksAndCategoriesFromRoleRequest request)
        {
            using (var client = GetRestClient())
            {
                client.RemoveBooksFromRole(request.RoleId, request.BookIds);
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
        public ActionResult AddSpecialPermissionsToRole([FromBody] AddSpecialPermissionsToRoleRequest request)
        {
            using (var client = GetRestClient())
            {
                client.AddSpecialPermissionsToRole(request.RoleId, new List<int>{request.SpecialPermissionId});
                return Json(new { });
            }
        }

        [HttpPost]
        public ActionResult RemoveSpecialPermissionsFromRole([FromBody] RemoveSpecialPermissionsFromRoleRequest request)
        {
            using (var client = GetRestClient())
            {
                client.RemoveSpecialPermissionsFromRole(request.RoleId, new List<int> { request.SpecialPermissionId});
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