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
using ITJakub.Web.Hub.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize]
    public class PermissionController : BaseController
    {
        private const int UserListPageSize = 10;
        private const int RoleListPageSize = 10;
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
                var model = CreateListViewModel<UserDetailViewModel, UserDetailContract>(result, start, count, search);

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
                    Start = start,
                    SearchQuery = search
                };

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

        public ActionResult RolePermissionList(int roleId, string search, int start, int count = PermissionListPageSize)
        {
            using (var client = GetRestClient())
            {
                try
                {
                    search = search ?? string.Empty;
                    var roleContract = client.GetRoleDetail(roleId);
                    var pagedPermissionsResult = client.GetPermissions(start, count, search);
                    var permissionList = Mapper.Map<List<PermissionViewModel>>(pagedPermissionsResult.List);

                    foreach (var permission in permissionList)
                    {
                        permission.IsSelected = roleContract.Permissions.Any(x => x.Id == permission.Id);
                    }

                    var model = new ListViewModel<PermissionViewModel>
                    {
                        TotalCount = pagedPermissionsResult.TotalCount,
                        List = permissionList,
                        PageSize = count,
                        Start = start,
                        SearchQuery = search
                    };

                    return PartialView("Widget/_PermissionListWidget", model);
                }
                catch (HttpErrorCodeException e)
                {
                    return BadRequest(new {e.Message});
                }
            }
        }

        public ActionResult UsersByRole(int roleId, string search, int start, int count = UserListPageSize)
        {
            try
            {
                using (var client = GetRestClient())
                {
                    search = search ?? string.Empty;
                    var result = client.GetUsersByRole(roleId, start, count, search);
                    var model = CreateListViewModel<UserDetailViewModel, UserContract>(result, start, count, search);
                    return PartialView("Widget/_UserListWidget", model);
                }
            }
            catch (HttpErrorCodeException e)
            {
                return BadRequest(new {e.Message});
            }
        }

        public ActionResult EditUser(int userId, bool successUpdate = false)
        {
            using (var client = GetRestClient())
            {
                if (successUpdate)
                {
                    ViewData.Add(AccountConstants.SuccessUserUpdate, true);
                }
                var result = client.GetUserDetail(userId);
                var model = Mapper.Map<UpdateUserViewModel>(result);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(UpdateUserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = GetRestClient())
                {
                    var data = new UpdateUserContract
                    {
                        FirstName = userViewModel.FirstName,
                        LastName = userViewModel.LastName
                    };

                    try
                    {
                        client.UpdateUser(userViewModel.Id, data);
                        return RedirectToAction("EditUser", new {userId = userViewModel.Id, successUpdate = true});
                    }
                    catch (HttpErrorCodeException e)
                    {
                        AddErrors(e);
                    }
                }
            }

            return View(userViewModel);
        }

        public ActionResult EditRole(int roleId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetRoleDetail(roleId);
                var model = Mapper.Map<RoleViewModel>(result);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(RoleViewModel roleViewModel)
        {
            using (var client = GetRestClient())
            {
                var roleContract = new RoleContract
                {
                    Id = roleViewModel.Id,
                    Name = roleViewModel.Name,
                    Description = roleViewModel.Description
                };
                client.UpdateRole(roleContract.Id, roleContract);

                var role = client.GetRoleDetail(roleContract.Id);
                var model = Mapper.Map<RoleViewModel>(role);
                model.SuccessfulUpdate = true;
                return View(model);
            }
        }

        public ActionResult EditUserRoles(int userId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetUserDetail(userId);
                var model = Mapper.Map<UserDetailViewModel>(result);
                model.Roles = new ListViewModel<RoleContract>
                {
                    TotalCount = result.Roles.Count,
                    List = result.Roles,
                    PageSize = RoleListPageSize,
                    Start = 0
                };

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

        public ActionResult GetTypeaheadRole(string query)
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

        public ActionResult GetRole(int roleId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetRoleDetail(roleId);
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult AddUserToRole([FromBody] AddUserToRoleRequest request)
        {
            try
            {
                using (var client = GetRestClient())
                {
                    client.AddUserToRole(request.UserId, request.RoleId);
                    return Json(new { });
                }
            }
            catch (HttpErrorCodeException e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [HttpPost]
        public ActionResult CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                using (var client = GetRestClient())
                {
                    var newRoleContract = new RoleContract
                    {
                        Name = request.RoleName,
                        Description = request.RoleDescription,
                    };
                    var roleId = client.CreateRole(newRoleContract);
                    var role = client.GetRoleDetail(roleId);
                    return Json(role);
                }
            }
            catch (HttpErrorCodeException e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [HttpPost]
        public ActionResult CreateRoleWithUser([FromBody] CreateRoleWithUserRequest request)
        {
            try
            {
                using (var client = GetRestClient())
                {
                    var roleId = client.CreateRole(new RoleContract
                    {
                        Name = request.RoleName,
                        Description = request.RoleDescription,
                    });
                    client.AddUserToRole(request.UserId, roleId);
                    return Json(roleId);
                }
            }
            catch (HttpErrorCodeException e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [HttpPost]
        public ActionResult RemoveUserFromRole([FromBody] RemoveUserFromRoleRequest request)
        {
            try
            {
                using (var client = GetRestClient())
                {
                    client.RemoveUserFromRole(request.UserId, request.RoleId);
                    return Json(new { });
                }
            }
            catch (HttpErrorCodeException e)
            {
                return BadRequest(new {e.Message});
            }
        }

        public ActionResult GetRolesByUser(int userId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetRolesByUser(userId);
                var model = new ListViewModel<RoleContract>
                {
                    TotalCount = result.Count,
                    List = result,
                    PageSize = result.Count,
                    Start = 0
                };

                return PartialView("Widget/_RoleListWidget", model);
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
            try
            {
                using (var client = GetRestClient())
                {
                    client.DeleteRole(request.RoleId);
                    return Json(new { });
                }
            }
            catch (HttpErrorCodeException e)
            {
                return BadRequest(new {e.Message});
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

        [HttpPost]
        public ActionResult AddSpecialPermissionsToRole([FromBody] AddSpecialPermissionsToRoleRequest request)
        {
            using (var client = GetRestClient())
            {
                try
                {
                    client.AddSpecialPermissionsToRole(request.RoleId, new List<int> {request.SpecialPermissionId});
                    return Json(new { });
                }
                catch (HttpErrorCodeException e)
                {
                    return BadRequest(new {e.Message});
                }
            }
        }

        [HttpPost]
        public ActionResult RemoveSpecialPermissionsFromRole([FromBody] RemoveSpecialPermissionsFromRoleRequest request)
        {
            using (var client = GetRestClient())
            {
                try
                {
                    client.RemoveSpecialPermissionsFromRole(request.RoleId, new List<int> {request.SpecialPermissionId});
                    return Json(new { });
                }
                catch (HttpErrorCodeException e)
                {
                    return BadRequest(new {e.Message});
                }
            }
        }

        private ListViewModel<TTarget> CreateListViewModel<TTarget, TSource>(PagedResultList<TSource> data, int start, int pageSize,
            string search)
        {
            return new ListViewModel<TTarget>
            {
                TotalCount = data.TotalCount,
                List = Mapper.Map<List<TTarget>>(data.List),
                PageSize = pageSize,
                Start = start,
                SearchQuery = search
            };
        }
    }
}