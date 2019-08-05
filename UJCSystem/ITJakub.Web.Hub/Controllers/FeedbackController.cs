using System.Collections.Generic;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(VokabularPermissionNames.ManageFeedbacks)]
    public class FeedbackController : BaseController
    {
        public FeedbackController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public ActionResult Feedback()
        {
            return View("FeedbackManagement");
        }

        public ActionResult GetFeedbacksCount(IList<FeedbackCategoryEnumContract> categories)
        {
            using (var client = GetRestClient())
            {
                var feedbacks = client.GetFeedbackList(0, 0, FeedbackSortEnumContract.Date, SortDirectionEnumContract.Desc, categories);
                return Json(feedbacks.TotalCount);
            }
        }

        public ActionResult GetFeedbacks(IList<FeedbackCategoryEnumContract> categories, int start, int count, byte sortCriteria, bool sortAsc)
        {
            var sortValue = (FeedbackSortEnumContract) sortCriteria;
            var sortDirection = sortAsc ? SortDirectionEnumContract.Asc : SortDirectionEnumContract.Desc;
            using (var client = GetRestClient())
            {
                var feedbacks = client.GetFeedbackList(start, count, sortValue, sortDirection, categories);
                return Json(feedbacks.List);
            }
        }

        [HttpPost]
        public ActionResult DeleteFeedback([FromBody] DeleteFeedbackRequest request)
        {
            using (var client = GetRestClient())
            {
                client.DeleteFeedback(request.FeedbackId);
                return Json(new {});
            }
        }
    }
}