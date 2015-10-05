using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Web.Hub.Identity;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(Roles = CustomRole.CanManagePermissions)]
    public class PermissionController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient = new ItJakubServiceClient();
        private readonly ItJakubServiceEncryptedClient m_mainServiceEncryptedClient = new ItJakubServiceEncryptedClient();


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
            var result = m_mainServiceClient.GetTypeaheadUsers(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }      
          
        public ActionResult GetTypeaheadGroup(string query)
        {
            var result = m_mainServiceClient.GetTypeaheadGroups(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
         
        public ActionResult GetUser(int userId)
        {
            var result = m_mainServiceClient.GetUserDetail(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGroup(int groupId)
        {
            var result = m_mainServiceClient.GetGroupDetail(groupId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUserToGroup(int userId, int groupId)
        {
            m_mainServiceClient.AddUserToGroup(userId, groupId);
            return Json(new {});
        }

        [HttpPost]
        public ActionResult CreateGroup(string groupName, string groupDescription)
        {
            var group = m_mainServiceClient.CreateGroup(groupName, groupDescription);
            return Json(group);
        }

        [HttpPost]
        public ActionResult CreateGroupWithUser(int userId, string groupName, string groupDescription)
        {
            var group = m_mainServiceClient.CreateGroup(groupName, groupDescription);
            m_mainServiceClient.AddUserToGroup(userId, group.Id);
            return Json(group);
        }

        [HttpPost]
        public ActionResult RemoveUserFromGroup(int userId, int groupId)
        {
            m_mainServiceClient.RemoveUserFromGroup(userId, groupId);
            return Json(new {});
        }

        public ActionResult GetGroupsByUser(int userId)
        {
            var result = m_mainServiceClient.GetGroupsByUser(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRootCategories()
        {
            var result = m_mainServiceClient.GetRootCategories();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetCategoryContent(int groupId, int categoryId)
        {
            var result = m_mainServiceClient.GetCategoryContentForGroup(groupId, categoryId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }     
           
        public ActionResult GetAllCategoryContent(int categoryId)
        {
            var result = m_mainServiceClient.GetAllCategoryContent(categoryId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
                   
        [HttpPost]
        public ActionResult DeleteGroup(int groupId)
        {
            m_mainServiceClient.DeleteGroup(groupId);
            return Json(new {});
        }
                           
        [HttpPost]
        public ActionResult AddBooksAndCategoriesToGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        {
            m_mainServiceClient.AddBooksAndCategoriesToGroup(groupId, bookIds, categoryIds);
            return Json(new {});
        }

        [HttpPost]
        public ActionResult RemoveBooksAndCategoriesFromGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        {
            m_mainServiceClient.RemoveBooksAndCategoriesFromGroup(groupId, bookIds, categoryIds);
            return Json(new {});
        }


        public ActionResult GetSpecialPermissionsForGroup(int groupId)
        {
            var specialPermissions = m_mainServiceClient.GetSpecialPermissionsForGroup(groupId);
            var result = specialPermissions.GroupBy(x => x.GetType().FullName).ToDictionary(x => x.Key, x => x.ToList());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSpecialPermissions()
        {
            var specialPermissions = m_mainServiceClient.GetSpecialPermissions();
            var result = specialPermissions.GroupBy(x => x.GetType().FullName).ToDictionary(x => x.Key, x => x.ToList());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddSpecialPermissionsToGroup(int groupId, IList<int> specialPermissionIds)
        {
            m_mainServiceClient.AddSpecialPermissionsToGroup(groupId, specialPermissionIds);
            return Json(new { });
        }

        [HttpPost]
        public ActionResult RemoveSpecialPermissionsFromGroup(int groupId, IList<int> specialPermissionIds)
        {
            m_mainServiceClient.RemoveSpecialPermissionsFromGroup(groupId, specialPermissionIds);
            return Json(new { });
        }
    }
}