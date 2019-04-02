using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Controllers
{
    [RequireHttps]
    [Authorize]
    [Area("RepositoryImport")]
    public class FilteringExpressionController : BaseController
    { 
        private const int FilteringExpressionSetCount = 5;

        public FilteringExpressionController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
          
        }


        public ActionResult FilteringExpressionSetList()
        {
            using (var client = GetRestClient())
            {
                var filteringExpressionSetList = client.GetFilteringExpressionSetList(0, FilteringExpressionSetCount, true);
                return View("FilteringExpressionSetList", filteringExpressionSetList);
            }
        }
    }
}