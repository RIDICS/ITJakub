using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITJakub.Shared.Contracts.Notes;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize]
    public class FeedbackController : BaseController
    {
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

            using (var client = GetAuthenticatedClient())
            {
                var count = client.GetFeedbacksCount(feedbackCriteria);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
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
            using (var client = GetAuthenticatedClient())
            {
                var results = client.GetFeedbacks(feedbackCriteria);
                return Json(results, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteFeedback(long feedbackId)
        {
            using (var client = GetAuthenticatedClient())
            {
                client.DeleteFeedback(feedbackId);
                return Json(new {});
            }
        }
    }
}