using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Types;
using HitSettingsContract = Vokabular.Shared.DataContracts.Search.OldCriteriaItem.HitSettingsContract;

namespace ITJakub.Web.Hub.Areas.Editions.Controllers
{
    [Area("GoldenLayout")]
    public class GoldenLayoutController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public GoldenLayoutController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager, CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.Edition;
        
        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification { Area = "GoldenLayout", Controller = "GoldenLayout" };
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
        /*
        // GET: Editions/Editions
        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
        {
            return View();
        }
        */
        
        public ActionResult Listing(long bookId, string searchText, string pageId)
        {
            using (var client = GetRestClient())
            {
                var book = client.GetBookInfo(bookId);
                var pages = client.GetBookPageList(bookId);
                var audioBookPage = client.GetAudioBookDetail(bookId).Tracks;
                return
                    View(new BookListingModel
                    {
                        BookId = book.Id,
                        SnapshotId = null, 
                        BookTitle = book.Title,
                        BookPages = pages,
                        AudioTracks = audioBookPage,
                        SearchText = searchText,
                        InitPageId = pageId, 
                        CanPrintEdition = User.IsInRole("CanEditionPrint"),
                        JsonSerializerSettingsForBiblModule = GetJsonSerializerSettingsForBiblModule()
                    });
            }
        }
        /*
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextEditionInfo);
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

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.Editions, IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult Help()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextEditionHelp);
            return View(pageStaticText);
        }

        public ActionResult EditionPrinciples()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextEditionPrinciples);
            return View(pageStaticText);
        }
        */

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

        public ActionResult AdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaJson(json, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = result }, GetJsonSerializerSettingsForBiblModule());
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
            var result = SearchByCriteriaText(CriteriaKey.Title, text, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = result }, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult TextSearchFulltextPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var results = SearchByCriteriaText(CriteriaKey.Fulltext, text, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = results }, GetJsonSerializerSettingsForBiblModule());
        }

        #endregion

        #region Search in book

        public ActionResult AdvancedSearchInBookPaged(string json, int start, int count, string bookXmlId, string versionXmlId)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = 0,
                Count = 1,
                HitSettingsContract = new HitSettingsContract
                {
                    ContextLength = 45,
                    Count = count,
                    Start = start
                }
            });

            listSearchCriteriaContracts.Add(new ResultRestrictionCriteriaContract
            {
                ResultBooks = new List<BookVersionPairContract> {new BookVersionPairContract {Guid = bookXmlId, VersionId = versionXmlId}}
            });
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
                if (result != null)
                {
                    return Json(new {results = result.Results}, GetJsonSerializerSettingsForBiblModule());
                }

                return Json(new {results = new PageResultContext[0]}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult AdvancedSearchInBookCount(string json, string bookXmlId, string versionXmlId)
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
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
                if (result != null)
                {
                    var count = result.TotalHitCount;
                    return Json(new {count});
                }

                return Json(new {count = 0});
            }
        }

        public ActionResult AdvancedSearchInBookPagesWithMatchHit(string json, long projectId, long? snapshotId)
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

                return Json(new {pages = result});
            }
        }

        public ActionResult TextSearchInBookPaged(string text, int start, int count, string bookXmlId, string versionXmlId)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = 0,
                Count = 1,
                HitSettingsContract = new HitSettingsContract
                {
                    ContextLength = 45,
                    Count = count,
                    Start = start
                }
            });
            listSearchCriteriaContracts.Add(new ResultRestrictionCriteriaContract
            {
                ResultBooks =
                    new List<BookVersionPairContract>
                    {
                        new BookVersionPairContract {Guid = bookXmlId, VersionId = versionXmlId}
                    }
            });

            using (var client = GetMainServiceClient())
            {
                var result = client.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
                if (result != null)
                {
                    return Json(new {results = result.Results}, GetJsonSerializerSettingsForBiblModule());
                }

                return Json(new {results = new PageResultContext[0]}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult TextSearchInBookCount(string text, string bookXmlId, string versionXmlId)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = 0,
                Count = 1
            });
            listSearchCriteriaContracts.Add(new ResultRestrictionCriteriaContract
            {
                ResultBooks =
                    new List<BookVersionPairContract>
                    {
                        new BookVersionPairContract {Guid = bookXmlId, VersionId = versionXmlId}
                    }
            });
            
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
                if (result != null)
                {
                    var count = result.TotalHitCount;
                    return Json(new {count});
                }

                return Json(new {count = 0});
            }
        }

        public ActionResult TextSearchInBookPagesWithMatchHit(string text, long projectId, long? snapshotId)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);
            
            using (var client = GetRestClient())
            {
                var request = new SearchPageRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts
                };
                var result = client.SearchPage(projectId, request);

                return Json(new {pages = result});
            }
        }

        #endregion
    }
}