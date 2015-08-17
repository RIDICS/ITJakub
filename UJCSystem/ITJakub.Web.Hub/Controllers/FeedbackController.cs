using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient = new ItJakubServiceClient();
        private readonly ItJakubServiceEncryptedClient m_mainServiceEncryptedClient = new ItJakubServiceEncryptedClient();

        public ActionResult Feedback()
        {
            return View();
        }

        public ActionResult GetFeedbacksCount()
        {
            return Json(0, JsonRequestBehavior.AllowGet);
        }
    }
}