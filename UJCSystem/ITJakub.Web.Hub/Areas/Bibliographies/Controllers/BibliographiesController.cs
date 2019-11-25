﻿using System.Collections.Generic;
using System.Linq;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Types;
using ITJakub.Web.Hub.Options;
using Vokabular.MainService.DataContracts.Contracts.Search;

namespace ITJakub.Web.Hub.Areas.Bibliographies.Controllers
{
    [LimitedAccess(PortalType.ResearchPortal)]
    [Area("Bibliographies")]
    public class BibliographiesController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public BibliographiesController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager,
            ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.BibliographicalItem;

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification {Area = "Bibliographies", Controller = "Bibliographies"};
        }

        public ActionResult Index()
        {
            return View("Search");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextBibliographiesInfo, "bibliographies");
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback,
                IsUserLoggedIn(), "home", User);
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                m_feedbacksManager.FillViewModel(model, StaticTexts.TextHomeFeedback, "home", GetFeedbackFormIdentification());
                return View(model);
            }

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.Bibliographies, IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public override ActionResult GetTypeaheadAuthor(string query)
        {
            var client = GetCodeListClient();
            var result = client.GetOriginalAuthorAutocomplete(query);
            var resultStringList = result.Select(x => $"{x.LastName} {x.FirstName}");
            return Json(resultStringList);
        }

        public override ActionResult GetTypeaheadTitle(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var client = GetMetadataClient();
            var result = client.GetTitleAutocomplete(query, null, GetDefaultProjectType(), selectedCategoryIds, selectedBookIds);
            return Json(result);
        }

        public ActionResult AdvancedSearchResultsCount(string json)
        {
            var count = SearchByCriteriaJsonCount(json, null, null, GetAdvancedSearchParameters());
            return Json(new {count});
        }

        public ActionResult AdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc)
        {
            var result = SearchByCriteriaJson(json, start, count, sortingEnum, sortAsc, null, null, GetAdvancedSearchParameters());
            return Json(new {results = result}, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult TextSearchCount(string text)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Title, text, null, null, GetAdvancedSearchParameters());
            return Json(new {count});
        }

        public ActionResult TextSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc)
        {
            var result = SearchByCriteriaText(CriteriaKey.Title, text, start, count, sortingEnum, sortAsc, null, null, GetAdvancedSearchParameters());
            return Json(new {results = result}, GetJsonSerializerSettingsForBiblModule());
        }

        private SearchAdvancedParametersContract GetAdvancedSearchParameters()
        {
            return new SearchAdvancedParametersContract
            {
                IncludeAdditionalProjectTypes = true,
                AdditionalProjectTypes = new List<ProjectTypeContract>
                {
                    ProjectTypeContract.Research,
                    ProjectTypeContract.Bibliography,
                    ProjectTypeContract.Community,
                }
            };
        }
    }
}