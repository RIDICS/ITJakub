using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.DataContracts;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using ITJakub.Web.Hub.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.BohemianTextBank.Controllers
{
    [Area("BohemianTextBank")]
    public class BohemianTextBankController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public BohemianTextBankController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager,
            CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.TextBank;

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification {Area = "BohemianTextBank", Controller = "BohemianTextBank"};
        }

        public ActionResult Index()
        {
            return View("List");
        }

        [LimitedAccess(PortalType.ResearchPortal)]
        public ActionResult Search()
        {
            return View();
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public ActionResult SearchCombined()
        {
            return View();
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public ActionResult SearchSingleLazyload()
        {
            return View();
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public ActionResult SearchSinglePaged()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextTextBankInfo, "textbank");
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

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.BohemianTextBank, IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult Help()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextTextBankHelp, "textbank");
            return View(pageStaticText);
        }

        public ActionResult GetCorpusWithCategories()
        {
            var result = GetBooksAndCategories();
            return Json(result);
        }

        #region Search book

        public ActionResult TextSearchCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Title, text, selectedBookIds, selectedCategoryIds);
            return Json(new {count});
        }

        public ActionResult TextSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaText(CriteriaKey.Title, text, start, count, sortingEnum, sortAsc, selectedBookIds,
                selectedCategoryIds);
            return Json(new {books = result}, GetJsonSerializerSettingsForBiblModule());
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
            return Json(new {results = result}, GetJsonSerializerSettingsForBiblModule());
        }

        #endregion


        #region Search corpus

        [LimitedAccess(PortalType.ResearchPortal)]
        public ActionResult TextSearchFulltextCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            var client = GetBookClient();
            var count = client.SearchCorpusCount(new CorpusSearchRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            }, GetDefaultProjectType());
            return Json(new {count});
        }

        [LimitedAccess(PortalType.ResearchPortal)]
        public ActionResult TextSearchFulltextPaged(string text, int start, int count, int contextLength, short sortingEnum, bool sortAsc,
            IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            var client = GetBookClient();
            var resultList = client.SearchCorpus(new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ContextLength = contextLength,
                ConditionConjunction = listSearchCriteriaContracts,
                // TODO is sorting required? is sorting possible?
            }, GetDefaultProjectType());
            return Json(new {results = resultList});
        }

        [LimitedAccess(PortalType.ResearchPortal)]
        public ActionResult AdvancedSearchCorpusResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var deserialized =
                JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            if (listSearchCriteriaContracts.FirstOrDefault(x =>
                    x.Key == CriteriaKey.Fulltext || x.Key == CriteriaKey.Heading || x.Key == CriteriaKey.Sentence ||
                    x.Key == CriteriaKey.TokenDistance) == null) //TODO add check on string values empty
            {
                throw new ArgumentException("search in text can't be ommited");
            }

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            var client = GetBookClient();
            var count = client.SearchCorpusCount(new CorpusSearchRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts,
            }, GetDefaultProjectType());
            return Json(new {count});
        }

        [LimitedAccess(PortalType.ResearchPortal)]
        public ActionResult AdvancedSearchCorpusPaged(string json, int start, int count, int contextLength, short sortingEnum, bool sortAsc,
            IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var deserialized =
                JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            if (listSearchCriteriaContracts.FirstOrDefault(x =>
                    x.Key == CriteriaKey.Fulltext || x.Key == CriteriaKey.Heading || x.Key == CriteriaKey.Sentence ||
                    x.Key == CriteriaKey.TokenDistance) == null) //TODO add check on string values empty
            {
                throw new ArgumentException("search in text can't be ommited");
            }

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            var client = GetBookClient();
            var resultList = client.SearchCorpus(new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ContextLength = contextLength,
                ConditionConjunction = listSearchCriteriaContracts,
                // TODO is sorting required? is sorting possible?
            }, GetDefaultProjectType());
            return Json(new {results = resultList});
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public ActionResult BasicSearchGetResultSnapshotListPageOfIdsWithoutResultNumbers(CorpusListGetPageContractBasic searchQuery)
        {
            var text = searchQuery.Text;
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var selectedBookIds = searchQuery.SelectedBookIds;
            var selectedCategoryIds = searchQuery.SelectedCategoryIds;
            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            var sortBooksBy = searchQuery.SortBooksBy;
            var sortDirection = searchQuery.SortDirection;
            var start = searchQuery.Start;
            var count = searchQuery.Count;

            return Json(GetCorpusSearchResultSnapshotList(new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ConditionConjunction = listSearchCriteriaContracts,
                Sort = sortBooksBy,
                SortDirection = sortDirection
            }));
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public CorpusSearchSnapshotsResultContract BasicSearchGetResultSnapshotListPageOfIdsWithResultNumbers(
            CorpusListGetPageContractBasic searchQuery)
        {
            var text = searchQuery.Text;
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var selectedBookIds = searchQuery.SelectedBookIds;
            var selectedCategoryIds = searchQuery.SelectedCategoryIds;
            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            var sortBooksBy = searchQuery.SortBooksBy;
            var sortDirection = searchQuery.SortDirection;
            var start = searchQuery.Start;
            var count = searchQuery.Count;

            return GetCorpusSearchResultSnapshotList(new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ConditionConjunction = listSearchCriteriaContracts,
                Sort = sortBooksBy,
                SortDirection = sortDirection,
                FetchNumberOfResults = true
            });
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        [HttpGet]
        public ActionResult BasicCorpusSearchGetResultsPage([FromQuery] CorpusLookupContractBasicSearch request)
        {
            var text = request.Text;
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var start = request.Start;
            var count = request.Count;
            var contextLength = request.ContextLength;
            var snapshotId = request.SnapshotId;

            return GetSearchCorpusInSnapshotResult(snapshotId, new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ContextLength = contextLength,
                ConditionConjunction = listSearchCriteriaContracts,
            });
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public ActionResult AdvancedSearchGetResultSnapshotListPageOfIdsWithoutResultNumbers(CorpusListGetPageContractAdvanced searchQuery)
        {
            var json = searchQuery.Json;
            var selectedBookIds = searchQuery.SelectedBookIds;
            var selectedCategoryIds = searchQuery.SelectedCategoryIds;
            List<SearchCriteriaContract> listSearchCriteriaContracts;

            var sortBooksBy = searchQuery.SortBooksBy;
            var sortDirection = searchQuery.SortDirection;
            var start = searchQuery.Start;
            var count = searchQuery.Count;
            try
            {
                listSearchCriteriaContracts = CreateTextCriteriaListFromJson(json, selectedBookIds, selectedCategoryIds);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }

            return Json(GetCorpusSearchResultSnapshotList(new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ConditionConjunction = listSearchCriteriaContracts,
                Sort = sortBooksBy,
                SortDirection = sortDirection
            }));
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public CorpusSearchSnapshotsResultContract AdvancedSearchGetResultSnapshotListPageOfIdsWithResultNumbers(
            CorpusListGetPageContractAdvanced searchQuery)
        {
            var json = searchQuery.Json;
            var selectedBookIds = searchQuery.SelectedBookIds;
            var selectedCategoryIds = searchQuery.SelectedCategoryIds;
            var listSearchCriteriaContracts = CreateTextCriteriaListFromJson(json, selectedBookIds, selectedCategoryIds);

            var sortBooksBy = searchQuery.SortBooksBy;
            var sortDirection = searchQuery.SortDirection;
            var start = searchQuery.Start;
            var count = searchQuery.Count;

            return GetCorpusSearchResultSnapshotList(new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ConditionConjunction = listSearchCriteriaContracts,
                Sort = sortBooksBy,
                SortDirection = sortDirection,
                FetchNumberOfResults = true
            });
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public ActionResult AdvancedCorpusSearchGetResultsPage(CorpusLookupContractAdvancedSearch request)
        {
            var json = request.Json;

            List<SearchCriteriaContract> listSearchCriteriaContracts;
            try
            {
                listSearchCriteriaContracts = CreateTextCriteriaListFromJson(json);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }

            var start = request.Start;
            var count = request.Count;
            var contextLength = request.ContextLength;
            var snapshotId = request.SnapshotId;

            return GetSearchCorpusInSnapshotResult(snapshotId, new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ContextLength = contextLength,
                ConditionConjunction = listSearchCriteriaContracts,
            });
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public ActionResult BasicSearchGetTotalResultNumber(CorpusSearchTotalResultCountBasic request)
        {
            var text = request.Text;
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var selectedSnapshotIds = request.SelectedSnapshotIds;
            var selectedCategoryIds = request.SelectedCategoryIds;
            AddCategoryCriteria(listSearchCriteriaContracts, selectedSnapshotIds, selectedCategoryIds);

            return GetTotalNumberOfCorpusSearchResults(new SearchRequestContractBase
            {
                ConditionConjunction = listSearchCriteriaContracts,
            });
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public ActionResult AdvancedSearchGetTotalResultNumber(CorpusSearchTotalResultCountAdvanced request)
        {
            List<SearchCriteriaContract> listSearchCriteriaContracts;

            try
            {
                var json = request.Json;
                var selectedSnapshotIds = request.SelectedSnapshotIds;
                var selectedCategoryIds = request.SelectedCategoryIds;
                listSearchCriteriaContracts = CreateTextCriteriaListFromJson(json, selectedSnapshotIds, selectedCategoryIds);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }

            return GetTotalNumberOfCorpusSearchResults(new SearchRequestContractBase
            {
                ConditionConjunction = listSearchCriteriaContracts,
            });
        }

        private List<SearchCriteriaContract> CreateTextCriteriaListFromJson(string json, IList<long> selectedBookIds = null,
            IList<int> selectedCategoryIds = null)
        {
            var deserialized =
                JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            if (listSearchCriteriaContracts.FirstOrDefault(x =>
                    x.Key == CriteriaKey.Fulltext || x.Key == CriteriaKey.Heading || x.Key == CriteriaKey.Sentence ||
                    x.Key == CriteriaKey.TokenDistance) == null) //TODO add check on string values empty
            {
                throw new ArgumentException("Search in text can't be ommited");
            }

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            return listSearchCriteriaContracts;
        }

        private ActionResult GetSearchCorpusInSnapshotResult(long snapshotId, CorpusSearchRequestContract request)
        {
            var client = GetBookClient();
            var result = client.SearchCorpusInSnapshot(snapshotId, request);
            return Json(new {results = result});
        }

        private CorpusSearchSnapshotsResultContract GetCorpusSearchResultSnapshotList(CorpusSearchRequestContract request)
        {
            var client = GetBookClient();
            var result = client.SearchCorpusGetSnapshotList(request, GetDefaultProjectType());
            return result;
        }

        private ActionResult GetTotalNumberOfCorpusSearchResults(SearchRequestContractBase request)
        {
            var client = GetBookClient();
            var result = client.SearchCorpusTotalResultCount(request, GetDefaultProjectType());
            return Json(new {totalCount = result});
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public ActionResult BasicSearchGetPagePositionInAllResultPages(int hitResultTotalStart, int compositionsPerPage,
            CorpusListLookupBasicSearchParams searchParams)
        {
            var bookId = 0L;

            var compositionListStart = 0;
            var currentResultsUpToThisBook = 0L;

            long LoadCompositionPage()
            {
                while (true)
                {
                    var query = new CorpusListGetPageContractBasic
                    {
                        Start = compositionListStart,
                        Count = compositionsPerPage,
                        SortBooksBy = searchParams.SortBooksBy,
                        SortDirection = searchParams.SortDirection,
                        Text = searchParams.Text,
                        SelectedBookIds = searchParams.SelectedBookIds,
                        SelectedCategoryIds = searchParams.SelectedCategoryIds
                    };
                    var numberOfResultsArray = BasicSearchGetResultSnapshotListPageOfIdsWithResultNumbers(query);

                    var compositionResultNumbers = numberOfResultsArray.SnapshotList;
                    foreach (var composition in compositionResultNumbers)
                    {
                        currentResultsUpToThisBook += composition.ResultCount;
                        if (hitResultTotalStart >= currentResultsUpToThisBook) continue;
                        bookId = composition.SnapshotId;
                        return currentResultsUpToThisBook - composition.ResultCount;
                    }

                    compositionListStart += compositionsPerPage;
                }
            }

            var resultsUpToThisCompositionStart = LoadCompositionPage();
            return Json(new
            {
                compositionListStart = compositionListStart,
                bookId = bookId,
                hitResultStart = (hitResultTotalStart - resultsUpToThisCompositionStart)
            });
        }

        [LimitedAccess(PortalType.CommunityPortal)]
        public ActionResult AdvancedSearchGetPagePositionInAllResultPages(int hitResultTotalStart, int compositionsPerPage,
            CorpusListLookupAdvancedSearchParams searchParams)
        {
            var bookId = 0L;

            var compositionListStart = 0;
            var currentResultsUpToThisBook = 0L;

            long LoadCompositionPage()
            {
                while (true)
                {
                    var query = new CorpusListGetPageContractAdvanced
                    {
                        Start = compositionListStart,
                        Count = compositionsPerPage,
                        SortBooksBy = searchParams.SortBooksBy,
                        SortDirection = searchParams.SortDirection,
                        Json = searchParams.Json,
                        SelectedBookIds = searchParams.SelectedBookIds,
                        SelectedCategoryIds = searchParams.SelectedCategoryIds
                    };
                    var numberOfResultsArray = AdvancedSearchGetResultSnapshotListPageOfIdsWithResultNumbers(query);
                    var compositionResultNumbers = numberOfResultsArray.SnapshotList;
                    foreach (var composition in compositionResultNumbers)
                    {
                        currentResultsUpToThisBook += composition.ResultCount;
                        if (hitResultTotalStart >= currentResultsUpToThisBook) continue;
                        bookId = composition.SnapshotId;
                        return currentResultsUpToThisBook - composition.ResultCount;
                    }

                    compositionListStart += compositionsPerPage;
                }
            }

            var resultsUpToThisCompositionStart = LoadCompositionPage();
            return Json(new
            {
                compositionListStart = compositionListStart,
                bookId = bookId,
                hitResultStart = (hitResultTotalStart - resultsUpToThisCompositionStart)
            });
        }

        #endregion
    }
}