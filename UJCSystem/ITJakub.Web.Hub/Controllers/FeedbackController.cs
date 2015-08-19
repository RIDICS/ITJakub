using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITJakub.Shared.Contracts.Notes;

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

        public ActionResult GetFeedbacksCount(IEnumerable<byte> categories)
        {
            var feedbackCriteria = new FeedbackCriteriaContract
            {
                Categories = categories?.Select(x => (FeedbackCategoryEnumContract) x).ToList()
            };

            var count = m_mainServiceClient.GetFeedbacksCount(feedbackCriteria);
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFeedbacks(IEnumerable<byte> categories, int? start, int? count, byte sortCriteria, bool sortAsc)
        {
            var feedbackCriteria = new FeedbackCriteriaContract
            {
                Start = start,
                Count = count,
                Categories = categories?.Select(x => (FeedbackCategoryEnumContract) x).ToList(),
                SortCriteria = new FeedbackSortCriteriaContract
                {
                    SortAsc = sortAsc,
                    SortByField = (FeedbackSortEnum) sortCriteria
                }
            };

            var results = m_mainServiceClient.GetFeedbacks(feedbackCriteria);
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