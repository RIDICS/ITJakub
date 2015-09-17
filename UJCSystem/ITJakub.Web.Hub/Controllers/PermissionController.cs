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
        
        public ActionResult GetTypeaheadUser(string query)
        {
            var result = m_mainServiceClient.GetTypeaheadUsers(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}