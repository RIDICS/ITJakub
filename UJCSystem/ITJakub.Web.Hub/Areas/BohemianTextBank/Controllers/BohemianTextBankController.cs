using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;//TODO remove
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
using Vokabular.MainService.DataContracts.Contracts.Search;
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

        public BohemianTextBankController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager, CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.TextBank;

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification { Area = "BohemianTextBank", Controller = "BohemianTextBank" };
        }

        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Searchnew()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextTextBankInfo);
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback, IsUserLoggedIn());
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

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.BohemianTextBank, IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult Help()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextTextBankHelp);
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
            return Json(new { count });
        }

        public ActionResult TextSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaText(CriteriaKey.Title, text, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = result }, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult AdvancedSearchResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaJsonCount(json, selectedBookIds, selectedCategoryIds);
            return Json(new { count });
        }

        public ActionResult AdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaJson(json, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { results = result }, GetJsonSerializerSettingsForBiblModule());
        }

        #endregion


        #region Search corpus

        public ActionResult TextSearchFulltextCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            using (var client = GetRestClient())
            {
                var count = client.SearchCorpusCount(new CorpusSearchRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts
                });
                return Json(new {count});
            }
        }
        
        public ActionResult TextSearchFulltextPaged(string text, int start, int count, int contextLength, short sortingEnum, bool sortAsc,
            IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            using (var client = GetRestClient())
            {
                var resultList = client.SearchCorpus(new CorpusSearchRequestContract
                {
                    Start = start,
                    Count = count,
                    ContextLength = contextLength,
                    ConditionConjunction = listSearchCriteriaContracts,
                    // TODO is sorting required? is sorting possible?
                });
                return Json(new {results = resultList});
            }
        }
        
        public ActionResult AdvancedSearchCorpusResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            if (listSearchCriteriaContracts.FirstOrDefault(x => x.Key == CriteriaKey.Fulltext || x.Key == CriteriaKey.Heading || x.Key == CriteriaKey.Sentence || x.Key == CriteriaKey.TokenDistance) == null) //TODO add check on string values empty
            {
                throw new ArgumentException("search in text can't be ommited");
            }

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            using (var client = GetRestClient())
            {
                var count = client.SearchCorpusCount(new CorpusSearchRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts,
                });
                return Json(new { count });
            }
        }

        public ActionResult AdvancedSearchCorpusPaged(string json, int start, int count, int contextLength, short sortingEnum, bool sortAsc, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            if (listSearchCriteriaContracts.FirstOrDefault(x => x.Key == CriteriaKey.Fulltext || x.Key == CriteriaKey.Heading || x.Key == CriteriaKey.Sentence || x.Key == CriteriaKey.TokenDistance) == null) //TODO add check on string values empty
            {
                throw new ArgumentException("search in text can't be ommited");
            }

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            using (var client = GetRestClient())
            {
                var resultList = client.SearchCorpus(new CorpusSearchRequestContract
                {
                    Start = start,
                    Count = count,
                    ContextLength = contextLength,
                    ConditionConjunction = listSearchCriteriaContracts,
                    // TODO is sorting required? is sorting possible?
                });
                return Json(new {results = resultList});
            }
        }

        [HttpGet]
        public ActionResult GetHitBookIdsPaged(string text, SortTypeEnumContract sortBooksBy, SortDirectionEnumContract sortDirection, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            return GetSearchResult(new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ConditionConjunction = listSearchCriteriaContracts,
                Sort = sortBooksBy,
                SortDirection = sortDirection,
            });
            
        }

        [HttpGet]
        public ActionResult TextSearchFulltextGetBookPage([FromQuery] CorpusLookupContractBasicSearch request)
        {
            var text = request.Text;
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var start = request.Start;
            var count = request.Count;
            var contextLength = request.ContextLength;
            var snapshotId = request.SnapshotId;

            return GetSearchResult(new CorpusSearchRequestContract
                {
                    Start = start,
                    Count = count,
                    ContextLength = contextLength,
                    ConditionConjunction = listSearchCriteriaContracts,
                }, snapshotId);
        }
        
        [HttpGet]
        public ActionResult AdvancedSearchGetHitBookIdsPaged(string json, SortTypeEnumContract sortBooksBy, SortDirectionEnumContract sortDirection, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = CreateTexCriteriaListFromJson(json, selectedBookIds, selectedCategoryIds);
            
            return GetSearchResult(new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ConditionConjunction = listSearchCriteriaContracts,
                Sort = sortBooksBy,
                SortDirection = sortDirection,
            });
        }

        [HttpGet]
        public ActionResult AdvancedSearchCorpusGetPage([FromQuery] CorpusLookupContractAdvancedSearch request)
        {
            var json = request.Json;
            var listSearchCriteriaContracts = CreateTexCriteriaListFromJson(json);

            var start = request.Start;
            var count = request.Count;
            var contextLength = request.ContextLength;
            var snapshotId = request.SnapshotId;

            return GetSearchResult(new CorpusSearchRequestContract
            {
                Start = start,
                Count = count,
                ContextLength = contextLength,
                ConditionConjunction = listSearchCriteriaContracts,
            }, snapshotId);
        }

        public ActionResult GetAllPages(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text can't be null in fulltext search");
            }

            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            return GetTotalNumberOfOccurances(new SearchRequestContractBase
            {
                ConditionConjunction = listSearchCriteriaContracts,
            });
        }

        public ActionResult AdvancedGetAllPages(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = CreateTexCriteriaListFromJson(json, selectedBookIds, selectedCategoryIds);

            return GetTotalNumberOfOccurances(new SearchRequestContractBase
            {
                ConditionConjunction = listSearchCriteriaContracts,
            });
        }

        private List<SearchCriteriaContract> CreateTexCriteriaListFromJson(string json, IList<long> selectedBookIds = null, IList<int> selectedCategoryIds = null)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            if (listSearchCriteriaContracts.FirstOrDefault(x => x.Key == CriteriaKey.Fulltext || x.Key == CriteriaKey.Heading || x.Key == CriteriaKey.Sentence || x.Key == CriteriaKey.TokenDistance) == null) //TODO add check on string values empty
            {
                throw new ArgumentException("Search in text can't be ommited");
            }

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            return listSearchCriteriaContracts;
        }


        private ActionResult GetSearchResult(CorpusSearchRequestContract request, long? snapshotId = null)
        {
            using (var client = GetRestClient())
            {
                if (snapshotId.HasValue)
                {
                    var result = client.SearchCorpusSnapshot(snapshotId.Value, request);
                    return Json(new { results = result });
                }
                else
                {
                    var result = client.SearchCorpusSnapshots(request);
                    return Json(new { list = result.SnapshotIds, totalCount = result.TotalCount });
                }
            }
        }

        private ActionResult GetTotalNumberOfOccurances(SearchRequestContractBase request)
        {
            using (var client = GetRestClient())
            {
                var result = client.SearchCorpusSnapshotsCount(request);
                return Json(new { totalCount = result });
            }
        }

        
        #endregion
    }
}