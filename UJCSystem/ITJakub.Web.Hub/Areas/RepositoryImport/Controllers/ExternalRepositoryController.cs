using System;
using System.Web;
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

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Controllers
{
    [RequireHttps]
    [Authorize]
    [Area("RepositoryImport")]
    public class ExternalRepositoryController : BaseController
    {
        private const int RepositoryCount = 5;
        private const string OaiPmh = "OaiPmh";

        public ExternalRepositoryController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public IActionResult List()
        {
            using (var client = GetRestClient())
            {
                var externalRepositories = client.GetExternalRepositoryList(0, RepositoryCount, true);
                return View(externalRepositories);
            }
        }

        public IActionResult Detail(int id)
        {
            using (var client = GetRestClient())
            {
                var externalRepositoryDetail = client.GetExternalRepositoryStatistics(id);
                return PartialView("_Detail", externalRepositoryDetail);
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
                ViewData["availableBibliographicFormats"] = availableBibliographicFormats;

                var externalRepositoryTypes = client.GetAllExternalRepositoryTypes();
                var availableExternalRepositoryTypes = new SelectList(externalRepositoryTypes,
                    nameof(BibliographicFormatContract.Id),
                    nameof(BibliographicFormatContract.Name));
                ViewData["availableExternalRepositoryTypes"] = availableExternalRepositoryTypes;
            }

            return View(new CreateExternalRepositoryViewModel());
        }

        [HttpPost]
        public IActionResult Create(CreateExternalRepositoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            using (var client = GetRestClient())
            {
                client.CreateExternalRepository(new ExternalRepositoryDetailContract
                {
                    Name = model.Name,
                    Description = model.Description,
                    License = model.License,
                    Url = model.Url,
                    Configuration = GetConfiguration(Request.Form),
                    BibliographicFormat = new BibliographicFormatContract {Id = model.BibliographicFormatId},
                    ExternalRepositoryType = new ExternalRepositoryTypeContract {Id = model.ExternalRepositoryTypeId}
                });
                return RedirectToAction("List");
            }
        }

        public IActionResult Update(int id)
        {
            using (var client = GetRestClient())
            {
                var externalRepositoryDetail = client.GetExternalRepositoryDetail(id);

                var bibliographicFormats = client.GetAllBibliographicFormats();
                var availableBibliographicFormats = new SelectList(bibliographicFormats,
                    nameof(BibliographicFormatContract.Id),
                    nameof(BibliographicFormatContract.Name),
                    externalRepositoryDetail.BibliographicFormat.Id);
                ViewData["availableBibliographicFormats"] = availableBibliographicFormats;

                var externalRepositoryTypes = client.GetAllExternalRepositoryTypes();
                var availableExternalRepositoryTypes = new SelectList(externalRepositoryTypes,
                    nameof(BibliographicFormatContract.Id),
                    nameof(BibliographicFormatContract.Name),
                    externalRepositoryDetail.ExternalRepositoryType.Id);
                ViewData["availableExternalRepositoryTypes"] = availableExternalRepositoryTypes;

                var model = new CreateExternalRepositoryViewModel
                {
                    Name = externalRepositoryDetail.Name,
                    Id = externalRepositoryDetail.Id,
                    Description = externalRepositoryDetail.Description,
                    License = externalRepositoryDetail.License,
                    Url = externalRepositoryDetail.Url,
                    BibliographicFormatId = externalRepositoryDetail.BibliographicFormat.Id,
                    ExternalRepositoryTypeId = externalRepositoryDetail.ExternalRepositoryType.Id,
                    Configration = externalRepositoryDetail.Configuration
                };

                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Update(CreateExternalRepositoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Update", model);
            }

            using (var client = GetRestClient())
            {
                client.UpdateExternalRepository(model.Id, new ExternalRepositoryDetailContract
                {
                    Name = model.Name,
                    Description = model.Description,
                    License = model.License,
                    Url = model.Url,
                    Configuration = GetConfiguration(Request.Form),
                    BibliographicFormat = new BibliographicFormatContract {Id = model.BibliographicFormatId},
                    ExternalRepositoryType = new ExternalRepositoryTypeContract {Id = model.ExternalRepositoryTypeId}
                });
                return RedirectToAction("List");
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            using (var client = GetRestClient())
            {
                client.DeleteExternalRepository(id);
                return RedirectToAction("List");
            }
        }

        public IActionResult LoadApiConfiguration(string api, string config)
        {
            switch (api)
            {
                case OaiPmh:
                    SetConfiguration(OaiPmh, config);
                    return PartialView("_OaiPmh");
                default:
                    throw new ArgumentException($"API type {api} cannot be found.");
            }
        }

        [HttpGet]
        public IActionResult OaiPmhConnect(string url, string config)
        {
            SetConfiguration(OaiPmh, config);
            using (var client = GetRestClient())
            {
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
        }

        private string GetConfiguration(IFormCollection requestForm)
        {
            switch (requestForm["apiType"])
            {
                case OaiPmh:
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
            switch (apiType)
            {
                case OaiPmh:
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