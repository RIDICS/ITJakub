using System.Linq;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Helpers;
using ITJakub.Web.Hub.Models.Requests.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.Shared.Const;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(Roles = CustomRole.CanManagePermissions)]
    public class PermissionController : BaseController
    {
        public PermissionController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public ActionResult UserPermission()
        {
            return View();
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
                var result = client.GetUserGroupAutocomplete(query);
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
                var result = client.GetUserGroupDetail(groupId);
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult AddUserToGroup([FromBody] AddUserToGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.AddUserToGroup(request.UserId, request.GroupId);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult CreateGroup([FromBody] CreateGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                var newUserGroupRequest = new UserGroupContract
                {
                    Name = request.GroupName,
                    Description = request.GroupDescription,
                };
                var group = client.CreateGroup(newUserGroupRequest);
                return Json(group);
            }
        }

        [HttpPost]
        public ActionResult CreateGroupWithUser([FromBody] CreateGroupWithUserRequest request)
        {
            using (var client = GetRestClient())
            {
                var groupId = client.CreateGroup(new UserGroupContract
                {
                    Name = request.GroupName,
                    Description = request.GroupDescription,
                });
                client.AddUserToGroup(request.UserId, groupId);
                return Json(groupId);
            }
        }

        [HttpPost]
        public ActionResult RemoveUserFromGroup([FromBody] RemoveUserFromGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.RemoveUserFromGroup(request.UserId, request.GroupId);
                return Json(new {});
            }
        }

        public ActionResult GetGroupsByUser(int userId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetGroupsByUser(userId);
                return Json(result);
            }
        }

        public ActionResult GetRootCategories()
        {
            using (var client = GetRestClient())
            {
                var result = client.GetBookTypeList();
                var convertedResult = result.Select(x => new CategoryContract
                {
                    Description = BookTypeHelper.GetCategoryName(x.Type),
                });
                return Json(convertedResult);
            }
        }

        public ActionResult GetCategoryContent(int groupId, int categoryId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetCategoryContentForGroup(groupId, categoryId);
                return Json(result);
            }
        }

        public ActionResult GetAllCategoryContent(int categoryId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetAllCategoryContent(categoryId);
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.DeleteGroup(request.GroupId);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult AddBooksAndCategoriesToGroup([FromBody] AddBooksAndCategoriesToGroupRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.AddBooksAndCategoriesToGroup(request.GroupId, request.BookIds, request.CategoryIds);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult RemoveBooksAndCategoriesFromGroup([FromBody] RemoveBooksAndCategoriesFromGroupRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.RemoveBooksAndCategoriesFromGroup(request.GroupId, request.BookIds, request.CategoryIds);
                return Json(new {});
            }
        }


        public ActionResult GetSpecialPermissionsForGroup(int groupId)
        {
            using (var client = GetRestClient())
            {
                var specialPermissions = client.GetSpecialPermissionsForGroup(groupId);
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
                client.AddSpecialPermissionsToGroup(request.GroupId, request.SpecialPermissionIds);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult RemoveSpecialPermissionsFromGroup([FromBody] RemoveSpecialPermissionsFromGroupRequest request)
        {
            using (var client = GetRestClient())
            {
                client.RemoveSpecialPermissionsFromGroup(request.GroupId, request.SpecialPermissionIds);
                return Json(new {});
            }
        }
    }
}