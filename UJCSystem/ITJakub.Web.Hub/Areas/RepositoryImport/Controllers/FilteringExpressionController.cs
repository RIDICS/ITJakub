using ITJakub.Web.Hub.Areas.RepositoryImport.Models;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vokabular.MainService.DataContracts.Contracts;

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

        public IActionResult FilteringExpressionSetList()
        {
            using (var client = GetRestClient())
            {
                var filteringExpressionSetList = client.GetFilteringExpressionSetList(0, FilteringExpressionSetCount, true);
                return View("FilteringExpressionSetList", filteringExpressionSetList);
            }
        }

        public IActionResult FilteringExpressionSetDetail(int id)
        {
            using (var client = GetRestClient())
            {
                var filteringExpressionSet = client.GetFilteringExpressionSetDetail(id);
                var bibliographicFormats = client.GetAllBibliographicFormats();
                var selectList = new SelectList(bibliographicFormats, nameof(BibliographicFormatContract.Id), nameof(BibliographicFormatContract.Name), filteringExpressionSet.BibliographicFormat.Id);
                var model = new FilteringSetViewModel{FilteringExpressionSet = filteringExpressionSet, AvailableBibliographicFormats = selectList};
                return View("FilteringExpressionSetDetail", model);
            }
        }
    }
}