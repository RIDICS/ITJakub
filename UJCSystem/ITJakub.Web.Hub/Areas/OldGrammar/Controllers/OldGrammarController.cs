using System.Collections.Generic;
using AutoMapper;
using ITJakub.Web.Hub.Areas.OldGrammar.Models;
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
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.OldGrammar.Controllers
{
    [Area("OldGrammar")]
    public class OldGrammarController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public OldGrammarController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager, CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.Grammar;

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification { Area = "OldGrammar", Controller = "OldGrammar" };
        }

        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult ListTerms()
        {
            using (var client = GetRestClient())
            {
                var termCategories = client.GetTermCategoriesWithTerms();
                return View(new TermCategoriesWithTermsModel
                {
                    TermCategories = termCategories
                });
            }
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextGrammarInfo);
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback, GetUserName());
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                m_feedbacksManager.FillViewModel(model, StaticTexts.TextHomeFeedback, GetFeedbackFormIdentification());
                return View(model);
            }

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.OldGrammar, IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult Help()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextGrammarHelp);
            return View(pageStaticText);
        }

        public ActionResult Listing(long bookId, string searchText, string page)
        {
            // TODO modify this method according to EditionsController.Listing()

            using (var client = GetRestClient())
            {
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
                        JsonSerializerSettingsForBiblModule = GetJsonSerializerSettingsForBiblModule()
                    });
            }
        }

        public ActionResult GetListConfiguration()
        {
            var fullPath = "~/Areas/OldGrammar/Content/BibliographyPlugin/list_configuration.json";
            return File(fullPath, "application/json", fullPath);
        }

        public ActionResult GetSearchConfiguration()
        {
            var fullPath = "~/Areas/OldGrammar/Content/BibliographyPlugin/search_configuration.json";
            return File(fullPath, "application/json", fullPath);
        }

        public ActionResult GetGrammarsWithCategories()
        {
            var result = GetBooksAndCategories();
            return Json(result);
        }


        public ActionResult AdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaJson(json, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = result }, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult AdvancedSearchResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaJsonCount(json, selectedBookIds, selectedCategoryIds);
            return Json(new { count });
        }

        public ActionResult TextSearchCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Title, text, selectedBookIds, selectedCategoryIds);
            return Json(new { count });
        }

        public ActionResult TextSearchFulltextCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Term, text, selectedBookIds, selectedCategoryIds);
            return Json(new { count });
        }

        public ActionResult TextSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaText(CriteriaKey.Title, text, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = result }, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult TextSearchFulltextPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaText(CriteriaKey.Term, text, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = result }, GetJsonSerializerSettingsForBiblModule());
        }

        #region search in boook

        public ActionResult TextSearchInBook(string text, long projectId, long? snapshotId)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Term, text);

            using (var client = GetRestClient())
            {
                var request = new SearchPageRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts
                };
                var result = client.SearchPage(projectId, request);
                return Json(new {results = result});
            }
        }

        public ActionResult AdvancedSearchInBook(string json, long projectId, long? snapshotId)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            using (var client = GetRestClient())
            {
                var request = new SearchPageRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts
                };
                var result = client.SearchPage(projectId, request);
                return Json(new {results = result});
            }
        }

        #endregion
    }
}