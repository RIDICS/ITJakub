using System.Linq;
using System.Web.Mvc;
using ITJakub.Web.Hub.Identity;
using ITJakub.Web.Hub.Models.Requests.Permission;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(Roles = CustomRole.CanManagePermissions)]
    public class PermissionController : BaseController
    {

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
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadUsers(query);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTypeaheadGroup(string query)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadGroups(query);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUser(int userId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetUserDetail(userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetGroup(int groupId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetGroupDetail(groupId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddUserToGroup(AddUserToGroupRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.AddUserToGroup(request.UserId, request.GroupId);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult CreateGroup(CreateGroupRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var group = client.CreateGroup(request.GroupName, request.GroupDescription);
                return Json(group);
            }
        }

        [HttpPost]
        public ActionResult CreateGroupWithUser(CreateGroupWithUserRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var group = client.CreateGroup(request.GroupName, request.GroupDescription);
                client.AddUserToGroup(request.UserId, group.Id);
                return Json(group);
            }
        }

        [HttpPost]
        public ActionResult RemoveUserFromGroup(RemoveUserFromGroupRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.RemoveUserFromGroup(request.UserId, request.GroupId);
                return Json(new {});
            }
        }

        public ActionResult GetGroupsByUser(int userId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetGroupsByUser(userId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRootCategories()
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetRootCategories();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCategoryContent(int groupId, int categoryId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetCategoryContentForGroup(groupId, categoryId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllCategoryContent(int categoryId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetAllCategoryContent(categoryId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteGroup(int groupId)
        {
            using (var client = GetMainServiceClient())
            {
                client.DeleteGroup(groupId);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult AddBooksAndCategoriesToGroup(AddBooksAndCategoriesToGroupRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.AddBooksAndCategoriesToGroup(request.GroupId, request.BookIds, request.CategoryIds);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult RemoveBooksAndCategoriesFromGroup(RemoveBooksAndCategoriesFromGroupRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.RemoveBooksAndCategoriesFromGroup(request.GroupId, request.BookIds, request.CategoryIds);
                return Json(new {});
            }
        }


        public ActionResult GetSpecialPermissionsForGroup(int groupId)
        {
            using (var client = GetMainServiceClient())
            {
                var specialPermissions = client.GetSpecialPermissionsForGroup(groupId);
                var result = specialPermissions.GroupBy(x => x.GetType().FullName).ToDictionary(x => x.Key, x => x.ToList());
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSpecialPermissions()
        {
            using (var client = GetMainServiceClient())
            {
                var specialPermissions = client.GetSpecialPermissions();
                var result = specialPermissions.GroupBy(x => x.GetType().FullName).ToDictionary(x => x.Key, x => x.ToList());
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddSpecialPermissionsToGroup(AddSpecialPermissionsToGroupRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.AddSpecialPermissionsToGroup(request.GroupId, request.SpecialPermissionIds);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult RemoveSpecialPermissionsFromGroup(RemoveSpecialPermissionsFromGroupRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.RemoveSpecialPermissionsFromGroup(request.GroupId, request.SpecialPermissionIds);
                return Json(new {});
            }
        }
    }
}