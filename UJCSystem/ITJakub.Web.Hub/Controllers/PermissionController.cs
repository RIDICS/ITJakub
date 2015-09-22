using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts.Clients;

namespace ITJakub.Web.Hub.Controllers
{
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
        
        public ActionResult GetCategoryContent(int categoryId)
        {
            var result = m_mainServiceClient.GetCategoryContent(categoryId);
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
    }
}