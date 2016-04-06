﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITJakub.Web.Hub.Identity;

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
        public ActionResult AddUserToGroup(int userId, int groupId)
        {
            using (var client = GetMainServiceClient())
            {
                client.AddUserToGroup(userId, groupId);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult CreateGroup(string groupName, string groupDescription)
        {
            using (var client = GetMainServiceClient())
            {
                var group = client.CreateGroup(groupName, groupDescription);
                return Json(group);
            }
        }

        [HttpPost]
        public ActionResult CreateGroupWithUser(int userId, string groupName, string groupDescription)
        {
            using (var client = GetMainServiceClient())
            {
                var group = client.CreateGroup(groupName, groupDescription);
                client.AddUserToGroup(userId, group.Id);
                return Json(group);
            }
        }

        [HttpPost]
        public ActionResult RemoveUserFromGroup(int userId, int groupId)
        {
            using (var client = GetMainServiceClient())
            {
                client.RemoveUserFromGroup(userId, groupId);
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
        public ActionResult AddBooksAndCategoriesToGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        {
            using (var client = GetMainServiceClient())
            {
                client.AddBooksAndCategoriesToGroup(groupId, bookIds, categoryIds);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult RemoveBooksAndCategoriesFromGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        {
            using (var client = GetMainServiceClient())
            {
                client.RemoveBooksAndCategoriesFromGroup(groupId, bookIds, categoryIds);
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
        public ActionResult AddSpecialPermissionsToGroup(int groupId, IList<int> specialPermissionIds)
        {
            using (var client = GetMainServiceClient())
            {
                client.AddSpecialPermissionsToGroup(groupId, specialPermissionIds);
                return Json(new {});
            }
        }

        [HttpPost]
        public ActionResult RemoveSpecialPermissionsFromGroup(int groupId, IList<int> specialPermissionIds)
        {
            using (var client = GetMainServiceClient())
            {
                client.RemoveSpecialPermissionsFromGroup(groupId, specialPermissionIds);
                return Json(new {});
            }
        }
    }
}