using ITJakub.Web.Hub.Areas.RepositoryImport.Models;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Controllers
{
    [RequireHttps]
    [Authorize]
    [Area("RepositoryImport")]
    public class FilteringExpressionSetController : BaseController
    {
        public FilteringExpressionSetController(CommunicationProvider communicationProvider, HttpErrorCodeTranslator httpErrorCodeTranslator) : base(
            communicationProvider, httpErrorCodeTranslator)
        {

        }

        public IActionResult List()
        {
            using (var client = GetRestClient())
            {
                var filteringExpressionSetList = client.GetAllFilteringExpressionSets();
                return View(filteringExpressionSetList);
            }
        }

        public IActionResult Detail(int id)
        {
            using (var client = GetRestClient())
            {
                var filteringExpressionSet = client.GetFilteringExpressionSetDetail(id);

                var bibliographicFormats = client.GetAllBibliographicFormats();
                var availableBibliographicFormats = new SelectList(bibliographicFormats, 
                    nameof(BibliographicFormatContract.Id),
                    nameof(BibliographicFormatContract.Name),
                    filteringExpressionSet.BibliographicFormat.Id);
                ViewData[RepositoryImportConstants.AvailableBibliographicFormats] = availableBibliographicFormats;

                var model = new CreateFilteringExpressionSetViewModel
                {
                    Name = filteringExpressionSet.Name,
                    FilteringExpressions = filteringExpressionSet.FilteringExpressions,
                    Id = filteringExpressionSet.Id,
                };
                return View(model);
            }
        }

        public IActionResult Create()
        {
            using (var client = GetRestClient())
            {
                var bibliographicFormats = client.GetAllBibliographicFormats();
                var availableBibliographicFormats = new SelectList(bibliographicFormats, 
                    nameof(BibliographicFormatContract.Id),
                    nameof(BibliographicFormatContract.Name));
                ViewData[RepositoryImportConstants.AvailableBibliographicFormats] = availableBibliographicFormats;
            }
            return View(new CreateFilteringExpressionSetViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateFilteringExpressionSetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            using (var client = GetRestClient())
            {
                client.CreateFilteringExpressionSet(new FilteringExpressionSetDetailContract
                {
                    Name = model.Name,
                    BibliographicFormat = new BibliographicFormatContract{Id = model.BibliographicFormatId}, 
                    FilteringExpressions = model.FilteringExpressions
                });
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(CreateFilteringExpressionSetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Detail", model);
            }

            using (var client = GetRestClient())
            {
                client.UpdateFilteringExpressionSet(model.Id, new FilteringExpressionSetDetailContract
                {
                    Name = model.Name,
                    BibliographicFormat = new BibliographicFormatContract { Id = model.BibliographicFormatId },
                    FilteringExpressions = model.FilteringExpressions
                });
                return RedirectToAction("List");
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            using (var client = GetRestClient())
            {
                client.DeleteFilteringExpressionSet(id);
                return RedirectToAction("List");
            }
        }

        public IActionResult AddFilteringExpressionRow()
        {
            return PartialView("_FilteringExpressionRow", new FilteringExpressionContract());
        }       
    }
}