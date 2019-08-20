using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.Web.Hub.Areas.RepositoryImport.Models;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;
using Vokabular.ProjectImport.Shared.Const;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Controllers
{
    [RequireHttps]
    [Authorize]
    [Area("RepositoryImport")]
    public class ExternalRepositoryController : BaseController
    {
        public ExternalRepositoryController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public IActionResult List()
        {
            var client = GetExternalRepositoryClient();
            var externalRepositories = client.GetAllExternalRepositories();
            return View(externalRepositories);
        }

        public IActionResult Detail(int id)
        {
            var client = GetExternalRepositoryClient();
            var externalRepositoryDetail = client.GetExternalRepositoryStatistics(id);
            return PartialView("_Detail", externalRepositoryDetail);
        }

        public IActionResult Create()
        {
            var client = GetFilteringExpressionSetClient();

            var bibliographicFormats = client.GetAllBibliographicFormats();
            var availableBibliographicFormats = new SelectList(bibliographicFormats,
                nameof(BibliographicFormatContract.Id),
                nameof(BibliographicFormatContract.Name));
            ViewData[RepositoryImportConstants.AvailableBibliographicFormats] = availableBibliographicFormats;

            var externalRepositoryClient = GetExternalRepositoryClient();
            var externalRepositoryTypes = externalRepositoryClient.GetAllExternalRepositoryTypes();
            var availableExternalRepositoryTypes = new SelectList(externalRepositoryTypes,
                nameof(BibliographicFormatContract.Id),
                nameof(BibliographicFormatContract.Name));
            ViewData[RepositoryImportConstants.AvailableExternalRepositoryTypes] = availableExternalRepositoryTypes;

            var filteringExpressionSets = client.GetAllFilteringExpressionSets();
            return View(new CreateExternalRepositoryViewModel()
                {FilteringExpressionSets = filteringExpressionSets.Select(x => new CheckBoxEntity(x.Id, x.Name)).ToList()});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateExternalRepositoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            var filteringExpressionSets = model.FilteringExpressionSets == null
                ? new List<FilteringExpressionSetContract>()
                : model.FilteringExpressionSets.Where(x => x.IsChecked)
                    .Select(x => new FilteringExpressionSetContract {Id = x.Id}).ToList();

            var client = GetExternalRepositoryClient();
            client.CreateExternalRepository(new ExternalRepositoryDetailContract
            {
                Name = model.Name,
                Description = model.Description,
                License = model.License,
                Url = model.Url,
                UrlTemplate = model.UrlTemplate,
                Configuration = GetConfiguration(Request.Form),
                BibliographicFormat = new BibliographicFormatContract {Id = model.BibliographicFormatId},
                ExternalRepositoryType = new ExternalRepositoryTypeContract {Id = model.ExternalRepositoryTypeId},
                FilteringExpressionSets = filteringExpressionSets
            });
            return RedirectToAction("List");
        }

        public IActionResult Update(int id)
        {
            var client = GetFilteringExpressionSetClient();
            var externalRepositoryClient = GetExternalRepositoryClient();

            var externalRepositoryDetail = externalRepositoryClient.GetExternalRepositoryDetail(id);

            var bibliographicFormats = client.GetAllBibliographicFormats();
            var availableBibliographicFormats = new SelectList(bibliographicFormats,
                nameof(BibliographicFormatContract.Id),
                nameof(BibliographicFormatContract.Name),
                externalRepositoryDetail.BibliographicFormat.Id);
            ViewData[RepositoryImportConstants.AvailableBibliographicFormats] = availableBibliographicFormats;

            var externalRepositoryTypes = externalRepositoryClient.GetAllExternalRepositoryTypes();
            var availableExternalRepositoryTypes = new SelectList(externalRepositoryTypes,
                nameof(BibliographicFormatContract.Id),
                nameof(BibliographicFormatContract.Name),
                externalRepositoryDetail.ExternalRepositoryType.Id);
            ViewData[RepositoryImportConstants.AvailableExternalRepositoryTypes] = availableExternalRepositoryTypes;

            var filteringExpressionSets = client.GetAllFilteringExpressionSets();
            var availableFilteringExpressionSets = filteringExpressionSets.Select(x =>
                new CheckBoxEntity(x.Id, x.Name, externalRepositoryDetail.FilteringExpressionSets.Any(y => y.Id == x.Id))).ToList();

            var model = new CreateExternalRepositoryViewModel
            {
                Name = externalRepositoryDetail.Name,
                Id = externalRepositoryDetail.Id,
                Description = externalRepositoryDetail.Description,
                License = externalRepositoryDetail.License,
                Url = externalRepositoryDetail.Url,
                UrlTemplate = externalRepositoryDetail.UrlTemplate,
                BibliographicFormatId = externalRepositoryDetail.BibliographicFormat.Id,
                ExternalRepositoryTypeId = externalRepositoryDetail.ExternalRepositoryType.Id,
                Configuration = externalRepositoryDetail.Configuration,
                FilteringExpressionSets = availableFilteringExpressionSets
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(CreateExternalRepositoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Update", model);
            }

            var client = GetExternalRepositoryClient();
            var config = GetConfiguration(Request.Form);
            client.UpdateExternalRepository(model.Id, new ExternalRepositoryDetailContract
            {
                Name = model.Name,
                Description = model.Description,
                License = model.License,
                Url = model.Url,
                UrlTemplate = model.UrlTemplate,
                Configuration = string.IsNullOrEmpty(config) ? model.Configuration : config,
                BibliographicFormat = new BibliographicFormatContract {Id = model.BibliographicFormatId},
                ExternalRepositoryType = new ExternalRepositoryTypeContract {Id = model.ExternalRepositoryTypeId},
                FilteringExpressionSets = model.FilteringExpressionSets.Where(x => x.IsChecked)
                    .Select(x => new FilteringExpressionSetContract {Id = x.Id}).ToList()
            });
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var client = GetExternalRepositoryClient();
            client.DeleteExternalRepository(id);
            return RedirectToAction("List");
        }

        public IActionResult LoadApiConfiguration(string api, string config)
        {
            switch (api)
            {
                case ExternalRepositoryTypeNameConstant.OaiPhm:
                    SetConfiguration(ExternalRepositoryTypeNameConstant.OaiPhm, config);
                    return PartialView("_OaiPmh");
                default:
                    throw new ArgumentException($"API type {api} cannot be found.");
            }
        }

        [HttpGet]
        public IActionResult OaiPmhConnect(string url, string config)
        {
            SetConfiguration(ExternalRepositoryTypeNameConstant.OaiPhm, config);
            var client = GetExternalRepositoryClient();
            var result = client.GetOaiPmhRepositoryInfo(url);
            var model = new OaiPmhConfigurationViewModel
            {
                AdminMails = result.AdminMails,
                Description = result.Description,
                Url = result.Url,
                Name = result.Name,
                MetadataFormats = result.MetadataFormats,
                Sets = result.Sets
            };

            return PartialView("_OaiPmhConfiguration", model);
        }

        private string GetConfiguration(IFormCollection requestForm)
        {
            switch (requestForm["apiType"])
            {
                case ExternalRepositoryTypeNameConstant.OaiPhm:
                    if (string.IsNullOrEmpty(Request.Form["OaiPmhMetadataFormat"]) ||
                        string.IsNullOrEmpty(Request.Form["OaiPmhSet"]) ||
                        string.IsNullOrEmpty(Request.Form["OaiPmhResourceUrl"]))
                        return null;

                    return JsonConvert.SerializeObject(new OaiPmhRepositoryConfigurationContract
                    {
                        DataFormat = Request.Form["OaiPmhMetadataFormat"],
                        SetName = Request.Form["OaiPmhSet"],
                        Url = Request.Form["OaiPmhResourceUrl"],
                    });
                default:
                    throw new ArgumentException($"API type {requestForm["apiType"]} cannot be found.");
            }
        }

        private void SetConfiguration(string apiType, string configuration)
        {
            if (string.IsNullOrEmpty(configuration))
            {
                return;
            }

            switch (apiType)
            {
                case ExternalRepositoryTypeNameConstant.OaiPhm:
                {
                    var config = JsonConvert.DeserializeObject<OaiPmhRepositoryConfigurationContract>(configuration);
                    ViewData["oaiPmhUrl"] = config.Url;
                    ViewData["selectedSet"] = config.SetName;
                    ViewData["selectedMetadataFormat"] = config.DataFormat;
                    break;
                }

                default:
                    throw new ArgumentException($"API type {apiType} cannot be found.");
            }
        }
    }
}