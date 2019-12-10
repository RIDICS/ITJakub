using System.Collections.Generic;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.DataContracts;
using ITJakub.Web.Hub.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(VokabularPermissionNames.ManageFeedbacks)]
    public class FeedbackController : BaseController
    {
        public FeedbackController(ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
        }

        public ActionResult Feedback()
        {
            return View("FeedbackManagement");
        }

        public ActionResult GetFeedbacksCount(IList<FeedbackCategoryEnumContract> categories)
        {
            var client = GetFeedbackClient();
            var feedbacks = client.GetFeedbackList(0, 0, FeedbackSortEnumContract.Date, SortDirectionEnumContract.Desc, categories, PortalTypeValue);
            return Json(feedbacks.TotalCount);
        }

        public ActionResult GetFeedbacks(IList<FeedbackCategoryEnumContract> categories, int start, int count, byte sortCriteria,
            bool sortAsc)
        {
            var sortValue = (FeedbackSortEnumContract) sortCriteria;
            var sortDirection = sortAsc ? SortDirectionEnumContract.Asc : SortDirectionEnumContract.Desc;
            var client = GetFeedbackClient();
            var feedbacks = client.GetFeedbackList(start, count, sortValue, sortDirection, categories, PortalTypeValue);
            var result = Mapper.Map<List<FeedbackExtendedContract>>(feedbacks.List);
            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteFeedback([FromBody] DeleteFeedbackRequest request)
        {
            var client = GetFeedbackClient();
            client.DeleteFeedback(request.FeedbackId);
            return Json(new { });
        }
    }
}