﻿using System.Collections.Generic;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;
using ITJakub.Web.Hub.Options;

namespace ITJakub.Web.Hub.Areas.ProfessionalLiterature.Controllers
{
    [LimitedAccess(PortalType.ResearchPortal)]
    [Area("ProfessionalLiterature")]
    public class ProfessionalLiteratureController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public ProfessionalLiteratureController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager,
            ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification {Area = "ProfessionalLiterature", Controller = "ProfessionalLiterature"};
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.ProfessionalLiterature;

        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Listing(long bookId, string searchText, string page)
        {
            // TODO modify this method according to EditionsController.Listing()

            var client = GetBookClient();
            var book = client.GetBookInfo(bookId);
            var pages = client.GetBookPageList(bookId);

            return
                View(new BookListingModel
                {
                    BookId = book.Id,
                    BookXmlId = book.Id.ToString(),
                    VersionXmlId = null,
                    BookTitle = book.Title,
                    BookPages = pages,
                    SearchText = searchText,
                    InitPageXmlId = page,
                    JsonSerializerSettingsForBiblModule = GetJsonSerializerSettingsForBiblModule()
                });
        }
        
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextProfessionalInfo, "professional");
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

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.ProfessionalLiterature, PortalTypeValue,IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult Help()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextProfessionalHelp, "professional");
            return View(pageStaticText);
        }

        public ActionResult GetProfessionalLiteratureWithCategories()
        {
            var result = GetBooksAndCategories();
            return Json(result);
        }

        public ActionResult AdvancedSearchResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaJsonCount(json, selectedBookIds, selectedCategoryIds);
            return Json(new {count});
        }

        public ActionResult AdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc,
            IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaJson(json, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new {books = result}, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult TextSearchCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Title, text, selectedBookIds, selectedCategoryIds);
            return Json(new {count});
        }

        public ActionResult TextSearchFulltextCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Fulltext, text, selectedBookIds, selectedCategoryIds);
            return Json(new {count});
        }

        public ActionResult TextSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaText(CriteriaKey.Title, text, start, count, sortingEnum, sortAsc, selectedBookIds,
                selectedCategoryIds);
            return Json(new {books = result}, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult TextSearchFulltextPaged(string text, int start, int count, short sortingEnum, bool sortAsc,
            IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaText(CriteriaKey.Fulltext, text, start, count, sortingEnum, sortAsc, selectedBookIds,
                selectedCategoryIds);
            return Json(new {books = result}, GetJsonSerializerSettingsForBiblModule());
        }

        #region Search in book

        public ActionResult AdvancedSearchInBookPaged(string json, int start, int count, long projectId, long? snapshotId)
        {
            var deserialized =
                JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);
            listSearchCriteriaContracts = GetOnlyPageCriteria(listSearchCriteriaContracts);

            var client = GetBookClient();
            var result = client.SearchHitsWithPageContext(projectId, new SearchHitsRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts,
                ContextLength = 45,
                Count = count,
                Start = start,
            });

            return Json(new {results = result}, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult AdvancedSearchInBookCount(string json, long projectId, long? snapshotId)
        {
            var deserialized =
                JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);
            listSearchCriteriaContracts = GetOnlyPageCriteria(listSearchCriteriaContracts);

            var client = GetBookClient();

            var resultCount = client.SearchHitsResultCount(projectId, new SearchHitsRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            });

            return Json(new {count = resultCount});
        }

        public ActionResult AdvancedSearchInBookPagesWithMatchHit(string json, long projectId, long? snapshotId)
        {
            var deserialized =
                JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            var client = GetBookClient();
            var request = new SearchPageRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            };
            var result = client.SearchPage(projectId, request);

            return Json(new {pages = result});
        }

        public ActionResult TextSearchInBookPaged(string text, int start, int count, long projectId, long? snapshotId)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var client = GetBookClient();
            var result = client.SearchHitsWithPageContext(projectId, new SearchHitsRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts,
                ContextLength = 45,
                Count = count,
                Start = start,
            });

            return Json(new {results = result}, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult TextSearchInBookCount(string text, long projectId, long? snapshotId)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var client = GetBookClient();
            var resultCount = client.SearchHitsResultCount(projectId, new SearchHitsRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            });

            return Json(new {count = resultCount});
        }

        public ActionResult TextSearchInBookPagesWithMatchHit(string text, long projectId, long? snapshotId)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var client = GetBookClient();
            var request = new SearchPageRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            };
            var result = client.SearchPage(projectId, request);

            return Json(new {pages = result});
        }

        #endregion
    }
}