using System.Collections.Generic;
using System.Linq;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Web.Hub.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(Roles = CustomRole.CanManageFeedbacks)]
    public class FeedbackController : BaseController
    {
        public FeedbackController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

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

            using (var client = GetMainServiceClient())
            {
                var count = client.GetFeedbacksCount(feedbackCriteria);
                return Json(count);
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
            using (var client = GetMainServiceClient())
            {
                var results = client.GetFeedbacks(feedbackCriteria);
                return Json(results);
            }
        }

        [HttpPost]
        public ActionResult DeleteFeedback(long feedbackId)
        {
            using (var client = GetMainServiceClient())
            {
                client.DeleteFeedback(feedbackId);
                return Json(new {});
            }
        }
    }
}