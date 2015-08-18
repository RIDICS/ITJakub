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
            var count = m_mainServiceClient.GetFeedbacksCount();
            return Json( count , JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFeedbacks()
        {
            var results = m_mainServiceClient.GetFeedbacks();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteFeedback(long feedbackId)
        {
            m_mainServiceClient.DeleteFeedback(feedbackId);
            return Json(new {});
        }
    }
}