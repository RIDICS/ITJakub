using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
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
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.OldCriteriaItem;
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
            using (var client = GetMainServiceClient())
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
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback, GetEncryptedClient(), GetUserName());
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

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.OldGrammar, GetMainServiceClient(), IsUserLoggedIn(), GetUserName());
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
                        BookXmlId = book.Id.ToString(),
                        VersionXmlId = null,
                        BookTitle = book.Title,
                        BookPages = pages,
                        SearchText = searchText,
                        InitPageXmlId = page,
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

        public ActionResult TextSearchInBook(string text, string bookXmlId, string versionXmlId)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                new WordListCriteriaContract
                {
                    Key = CriteriaKey.Term,
                    Disjunctions = new List<WordCriteriaContract>
                    {
                        new WordCriteriaContract
                        {
                            Contains = new List<string> {text}
                        }
                    }
                },
                new ResultCriteriaContract
                {
                    Start = 0,
                    Count = 1
                },
                new ResultRestrictionCriteriaContract
                {
                    ResultBooks =
                        new List<BookVersionPairContract>
                        {
                            new BookVersionPairContract {Guid = bookXmlId, VersionId = versionXmlId}
                        }
                }
            };

            if (!string.IsNullOrWhiteSpace(text))
            {
                listSearchCriteriaContracts.OfType<ResultCriteriaContract>().First().TermsSettingsContract = new TermsSettingsContract();
            }
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
                if (result != null)
                {
                    return Json(new {results = result.TermsPageHits}, GetJsonSerializerSettingsForBiblModule());
                }

                return Json(new {}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult AdvancedSearchInBook(string json, string bookXmlId, string versionXmlId)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = 0,
                Count = 1
            });

            listSearchCriteriaContracts.Add(new ResultRestrictionCriteriaContract
            {
                ResultBooks = new List<BookVersionPairContract> {new BookVersionPairContract {Guid = bookXmlId, VersionId = versionXmlId}}
            });

            if (!string.IsNullOrWhiteSpace(json))
            {
                listSearchCriteriaContracts.OfType<ResultCriteriaContract>().First().TermsSettingsContract = new TermsSettingsContract();
            }
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
                if (result != null)
                {
                    return Json(new {results = result.TermsPageHits}, GetJsonSerializerSettingsForBiblModule());
                }

                return Json(new {}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        #endregion
    }
}