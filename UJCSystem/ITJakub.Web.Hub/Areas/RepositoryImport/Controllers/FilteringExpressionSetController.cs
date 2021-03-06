﻿using ITJakub.Web.Hub.Areas.RepositoryImport.Models;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vokabular.MainService.DataContracts.Contracts.ExternalBibliography;
using Vokabular.Shared.Const;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Controllers
{
    [RequireHttps]
    [LimitedAccess(PortalType.ResearchPortal)]
    [Authorize(VokabularPermissionNames.ManageBibliographyImport)]
    [Area("RepositoryImport")]
    public class FilteringExpressionSetController : BaseController
    {
        public FilteringExpressionSetController(ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
        }

        public IActionResult List()
        {
            var client = GetFilteringExpressionSetClient();
            var filteringExpressionSetList = client.GetAllFilteringExpressionSets();
            return View(filteringExpressionSetList);
        }

        public IActionResult Detail(int id)
        {
            var client = GetFilteringExpressionSetClient();
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

        public IActionResult Create()
        {
            var client = GetFilteringExpressionSetClient();
            var bibliographicFormats = client.GetAllBibliographicFormats();
            var availableBibliographicFormats = new SelectList(bibliographicFormats,
                nameof(BibliographicFormatContract.Id),
                nameof(BibliographicFormatContract.Name));
            ViewData[RepositoryImportConstants.AvailableBibliographicFormats] = availableBibliographicFormats;

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

            var client = GetFilteringExpressionSetClient();
            client.CreateFilteringExpressionSet(new FilteringExpressionSetDetailContract
            {
                Name = model.Name,
                BibliographicFormat = new BibliographicFormatContract {Id = model.BibliographicFormatId},
                FilteringExpressions = model.FilteringExpressions
            });
            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(CreateFilteringExpressionSetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Detail", model);
            }

            var client = GetFilteringExpressionSetClient();
            client.UpdateFilteringExpressionSet(model.Id, new FilteringExpressionSetDetailContract
            {
                Name = model.Name,
                BibliographicFormat = new BibliographicFormatContract {Id = model.BibliographicFormatId},
                FilteringExpressions = model.FilteringExpressions
            });
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var client = GetFilteringExpressionSetClient();
            client.DeleteFilteringExpressionSet(id);
            return RedirectToAction("List");
        }

        public IActionResult AddFilteringExpressionRow()
        {
            return PartialView("_FilteringExpressionRow", new FilteringExpressionContract());
        }
    }
}