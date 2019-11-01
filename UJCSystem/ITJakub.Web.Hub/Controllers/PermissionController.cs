﻿using System.Collections.Generic;
using System.Linq;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Constants;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Requests.Permission;
using ITJakub.Web.Hub.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts;
using Ridics.Core.Structures.Shared;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Errors;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize]
    public class PermissionController : BaseController
    {
        private const int UserListPageSize = 10;
        private const int RoleListPageSize = 10;
        private const int PermissionListPageSize = 10;
        private const int BookListPageSize = 10;

        public PermissionController(ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
        }

        public IActionResult UserPermission(string search, int start, int count = UserListPageSize, ViewType viewType = ViewType.Full)
        {
            var client = GetUserClient();

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

        public IActionResult RolePermission(string search, int start, int count = RoleListPageSize, ViewType viewType = ViewType.Full)
        {
            var client = GetRoleClient();

            search = search ?? string.Empty;
            var result = client.GetRoleList(start, count, search);
            var model = new ListViewModel<UserGroupContract>
            {
                TotalCount = result.TotalCount,
                List = result.List.Select(x => new UserGroupContract
                {
                    Id = x.Id,
                    ExternalId = x.ExternalId,
                    Name = x.Name,
                    Description = x.Description,
                    Type = UserGroupTypeContract.Role,
                }).ToList(),
                PageSize = count,
                Start = start,
                SearchQuery = search
            };

            ViewData.Add(RoleViewConstants.IsRoleEditAllowed, true);
            ViewData.Add(RoleViewConstants.UnregisteredRoleName, RoleNames.Unregistered);
            ViewData.Add(RoleViewConstants.RegisteredRoleName, RoleNames.RegisteredUser);

            switch (viewType)
            {
                case ViewType.Widget:
                    return PartialView("Widget/_RoleListWidget", model);
                case ViewType.Full:
                    return View(model);
                default:
                    return View(model);
            }
        }

        public ActionResult ProjectPermission(string search, int start, int count = BookListPageSize, ViewType viewType = ViewType.Full)
        {
            search = search ?? string.Empty;

            var client = GetProjectClient();
            var result = client.GetProjectList(start, count, GetDefaultProjectType(), search);
            var model = new ListViewModel<ProjectDetailContract>
            {
                TotalCount = result.TotalCount,
                List = result.List,
                PageSize = count,
                Start = start,
                SearchQuery = search
            };

            switch (viewType)
            {
                case ViewType.Widget:
                    return PartialView("Widget/_ProjectListWidget", model);
                case ViewType.Full:
                    return View(model);
                default:
                    return View(model);
            }
        }

        public IActionResult RolePermissionList(int roleId, string search, int start, int count = PermissionListPageSize)
        {
            search = search ?? string.Empty;

            var roleClient = GetRoleClient();
            var roleContract = roleClient.GetUserGroupDetail(roleId);
            var permissionClient = GetPermissionClient();

            var pagedPermissionsResult = permissionClient.GetPermissions(start, count, search);
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

        public IActionResult UsersByRole(int roleId, string search, int start, int count = UserListPageSize)
        {
            var client = GetRoleClient();
            search = search ?? string.Empty;
            var result = client.GetUsersByGroup(roleId, start, count, search);
            var model = CreateListViewModel<UserDetailViewModel, UserContract>(result, start, count, search);
            return PartialView("Widget/_UserListWidget", model);
        }

        public ActionResult RolesByProject(int projectId, string search, int start, int count = RoleListPageSize)
        {
            var client = GetProjectClient();
            search = search ?? string.Empty;
            var result = client.GetUserGroupsByProject(projectId, start, count, search);
            var model = new ListViewModel<UserGroupContract>
            {
                TotalCount = result.TotalCount,
                List = result.List,
                PageSize = count,
                Start = start,
                SearchQuery = search
            };

            ViewData[RoleViewConstants.IsSingleUserGroupRemoveAllowed] = true;
            return PartialView("Widget/_RoleListWidget", model);
        }

        public ActionResult EditUser(int userId, bool successUpdate = false)
        {
            ViewData.Add(AccountConstants.SuccessUserUpdate, successUpdate);
            
            var client = GetUserClient();
            var result = client.GetUserDetail(userId);
            var model = Mapper.Map<UpdateUserViewModel>(result);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(UpdateUserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var data = new UpdateUserContract
                {
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName
                };

                try
                {
                    var client = GetUserClient();
                    client.UpdateUser(userViewModel.Id, data);
                    return RedirectToAction("EditUser", new {userId = userViewModel.Id, successUpdate = true});
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
                catch (MainServiceException e)
                {
                    AddErrors(e);
                }
            }

            return View(userViewModel);
        }

        [HttpPost]
        public IActionResult ResetUserPassword([FromBody] ResetUserPasswordRequest request)
        {
            var client = GetUserClient();
            client.ResetUserPassword(request.UserId);
            return AjaxOkResponse();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRole(RoleViewModel roleViewModel)
        {
            roleViewModel.SuccessfulUpdate = false;
            if (ModelState.IsValid)
            {
                try
                {
                    var roleContract = new RoleContract
                    {
                        Id = roleViewModel.Id,
                        Name = roleViewModel.Name,
                        Description = roleViewModel.Description
                    };
                    var client = GetRoleClient();
                    client.UpdateRole(roleContract.Id, roleContract);
                    roleViewModel.SuccessfulUpdate = true;
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
                catch (MainServiceException e)
                {
                    AddErrors(e);
                }
            }

            return PartialView("_EditRole", roleViewModel);
        }

        public IActionResult EditUserRoles(int userId)
        {
            var client = GetUserClient();
            var resultUser = client.GetUserDetail(userId);
            var resultRoles = client.GetUserGroupsByUser(userId);
            var model = Mapper.Map<UserDetailViewModel>(resultUser);
            model.Roles = new ListViewModel<UserGroupContract>
            {
                TotalCount = resultRoles.Count,
                List = resultRoles,
                PageSize = RoleListPageSize,
                Start = 0
            };

            return View(model);
        }

        public IActionResult GetTypeaheadUser(string query)
        {
            var client = GetUserClient();
            var result = client.GetUserAutocomplete(query);
            return Json(result);
        }

        public IActionResult GetTypeaheadRole(string query)
        {
            var client = GetRoleClient();
            var result = client.GetRoleAutocomplete(query);
            return Json(result);
        }

        [HttpPost]
        public IActionResult AddUserToRole([FromBody] AddUserToRoleRequest request)
        {
            var client = GetRoleClient();
            client.AddUserToRole(request.UserId, request.RoleId);
            return AjaxOkResponse();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRole(RoleViewModel roleViewModel)
        {
            roleViewModel.SuccessfulUpdate = false;
            if (ModelState.IsValid)
            {
                try
                {
                    var roleContract = new RoleContract
                    {
                        Id = roleViewModel.Id,
                        Name = roleViewModel.Name,
                        Description = roleViewModel.Description
                    };
                    var client = GetRoleClient();
                    client.CreateRole(roleContract);
                    roleViewModel.SuccessfulUpdate = true;
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
                catch (MainServiceException e)
                {
                    AddErrors(e);
                }
            }

            return PartialView("_CreateRole", roleViewModel);
        }

        [HttpPost]
        public IActionResult CreateRoleWithUser([FromBody] CreateRoleWithUserRequest request)
        {
            var client = GetRoleClient();

            var roleId = client.CreateRole(new RoleContract
            {
                Name = request.RoleName,
                Description = request.RoleDescription,
            });
            client.AddUserToRole(request.UserId, roleId);
            return Json(roleId);
        }

        [HttpPost]
        public IActionResult RemoveUserFromRole([FromBody] RemoveUserFromRoleRequest request)
        {
            var client = GetRoleClient();
            client.RemoveUserFromRole(request.UserId, request.RoleId);
            return AjaxOkResponse();
        }

        public IActionResult GetRolesByUser(int userId)
        {
            var client = GetUserClient();
            var result = client.GetUserGroupsByUser(userId);
            var model = new ListViewModel<UserGroupContract>
            {
                TotalCount = result.Count,
                List = result,
                PageSize = result.Count,
                Start = 0
            };

            return PartialView("Widget/_RoleListWidget", model);
        }

        [HttpPost]
        public IActionResult DeleteRole([FromBody] DeleteRoleRequest request)
        {
            var client = GetRoleClient();
            client.DeleteRole(request.RoleId);
            return AjaxOkResponse();
        }

        [HttpPost]
        public IActionResult AddProjectsToRole([FromBody] AddProjectsToRoleRequest request)
        {
            var client = GetRoleClient();
            client.UpdateOrAddBooksToGroup(request.RoleId, request.BookId, new PermissionDataContract
            {
                ShowPublished = request.ShowPublished,
                ReadProject = request.ReadProject,
                AdminProject = request.AdminProject,
                EditProject = request.EditProject,
            });
            return AjaxOkResponse();
        }

        public IActionResult GetPermissionsForRoleAndBook(int roleId, long bookId)
        {
            var client = GetRoleClient();
            var result = client.GetPermissionsForGroupAndBook(roleId, bookId);
            return PartialView("Widget/_ProjectPermissionsWidget", result);
        }

        [HttpPost]
        public IActionResult RemoveProjectsFromRole([FromBody] RemoveProjectsFromRoleRequest request)
        {
            var client = GetRoleClient();
            client.RemoveBooksFromGroup(request.RoleId, request.BookId);
            return AjaxOkResponse();
        }

        [HttpPost]
        public IActionResult AddSpecialPermissionsToRole([FromBody] AddSpecialPermissionsToRoleRequest request)
        {
            var client = GetRoleClient();
            client.AddSpecialPermissionsToRole(request.RoleId, new List<int> {request.SpecialPermissionId});
            return AjaxOkResponse();
        }

        [HttpPost]
        public IActionResult RemoveSpecialPermissionsFromRole([FromBody] RemoveSpecialPermissionsFromRoleRequest request)
        {
            var client = GetRoleClient();
            client.RemoveSpecialPermissionsFromRole(request.RoleId, new List<int> {request.SpecialPermissionId});
            return Json(new { });
        }
    }
}