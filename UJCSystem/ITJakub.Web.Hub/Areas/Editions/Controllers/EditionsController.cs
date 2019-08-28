using System.Collections.Generic;
using AutoMapper;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.AspNetCore.Extensions;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Editions.Controllers
{
    [Area("Editions")]
    public class EditionsController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public EditionsController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager,
            CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.Edition;

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification {Area = "Editions", Controller = "Editions"};
        }

        public ActionResult GetListConfiguration()
        {
            var fullPath = "~/Areas/Editions/content/BibliographyPlugin/list_configuration.json";
            return File(fullPath, "application/json", fullPath);
        }

        public ActionResult GetSearchConfiguration()
        {
            var fullPath = "~/Areas/Editions/content/BibliographyPlugin/search_configuration.json";
            return File(fullPath, "application/json", fullPath);
        }


        #region Views and Feedback

        // GET: Editions/Editions
        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
        {
            return View();
        }

        // TODO rename parameter page to pageId
        public ActionResult Listing(long bookId, string searchText, string page)
        {
            var client = GetBookClient();
            var book = client.GetBookInfo(bookId);
            var pages = client.GetBookPageList(bookId);
            return
                View(new BookListingModel
                {
                    BookId = book.Id,
                        SnapshotId = null, 
                        BookTitle = book.Title,
                        BookPages = pages,
                        SearchText = searchText,
                        InitPageId = page, 
                        CanPrintEdition = User.HasPermission(VokabularPermissionNames.EditionPrintText),
                        JsonSerializerSettingsForBiblModule = GetJsonSerializerSettingsForBiblModule()
                });
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextEditionInfo, "edition");
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

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.Editions, IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult Help()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextEditionHelp, "edition");
            return View(pageStaticText);
        }

        public ActionResult EditionPrinciples()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextEditionPrinciples, "edition");
            return View(pageStaticText);
        }

        #endregion

        public ActionResult GetEditionsWithCategories()
        {
            var result = GetBooksAndCategories();
            return Json(result);
        }


        #region Search book

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
            var results = SearchByCriteriaText(CriteriaKey.Fulltext, text, start, count, sortingEnum, sortAsc, selectedBookIds,
                selectedCategoryIds);
            return Json(new {books = results}, GetJsonSerializerSettingsForBiblModule());
        }

        #endregion

        #region Search in book

        public ActionResult AdvancedSearchInBookPaged(string json, int start, int count, long projectId, long? snapshotId)
        {
            var deserialized =
                JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

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