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
                SetAvailableBibliographicFormats(filteringExpressionSet.BibliographicFormat.Id);
                var model = new CreateFilteringExpressionSetViewModel
                {
                    Name = filteringExpressionSet.Name,
                    FilteringExpressions = filteringExpressionSet.FilteringExpressions,
                    Id = filteringExpressionSet.Id,
                };
                return View("FilteringExpressionSetDetail", model);
            }
        }

        public IActionResult CreateSet(CreateFilteringExpressionSetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetAvailableBibliographicFormats(model.BibliographicFormatId);
                return View("FilteringExpressionSetDetail", model);
            }

            using (var client = GetRestClient())
            {
                client.CreateFilteringExpressionSet(new FilteringExpressionSetDetailContract
                {
                    Name = model.Name,
                    BibliographicFormat = new BibliographicFormatContract{Id = model.BibliographicFormatId}, 
                    FilteringExpressions = model.FilteringExpressions
                });
                return RedirectToAction("FilteringExpressionSetList");
            }
        }

        public IActionResult UpdateSet(CreateFilteringExpressionSetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("FilteringExpressionSetDetail", model);
            }

            using (var client = GetRestClient())
            {
                client.UpdateFilteringExpressionSet(model.Id, new FilteringExpressionSetDetailContract
                {
                    Name = model.Name,
                    BibliographicFormat = new BibliographicFormatContract { Id = model.BibliographicFormatId },
                    FilteringExpressions = model.FilteringExpressions
                });
                return RedirectToAction("FilteringExpressionSetList");
            }
        }

        public IActionResult AddFilteringExpressionRow()
        {
            return PartialView("_FilteringExpressionRow", new FilteringExpressionContract());
        }

        private void SetAvailableBibliographicFormats(int selectBibliographicFormatId)
        {
            using (var client = GetRestClient())
            {
                var bibliographicFormats = client.GetAllBibliographicFormats();
                var selectList = new SelectList(bibliographicFormats, nameof(BibliographicFormatContract.Id),
                    nameof(BibliographicFormatContract.Name), selectBibliographicFormatId);
                ViewData["selectList"] = selectList;
            }
        }
    }
}