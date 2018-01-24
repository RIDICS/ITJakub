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
using Vokabular.Shared.DataContracts.Search.RequestContracts;
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

        [HttpGet]
        public ActionResult GetHitBookIdsPaged(string text, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)//TODO mock, should return array of ids of books where results ocurred
        {
            using (var client = GetRestClient())
            {
                var ids = new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                var ids2 = new List<long> { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
                var ids3 = new List<long> { 21, 22, 23, 24 };
                var totalCount = 3;
                if (start<10)
                {
                    totalCount = 24;
                    return Json(new { list = ids, totalCount = totalCount });
                }
                if (start >= 10 && start <20)
                {
                    totalCount = 24;
                    return Json(new { list = ids2, totalCount = totalCount });
                }
                if (start >= 20 && start < 30)
                {
                    totalCount = 24;
                    return Json(new { list = ids3, totalCount = totalCount });
                }
                ids = null;
                totalCount = 24;
                return Json(new { list = ids, totalCount = totalCount });
            }
        }

        [HttpGet]
        public ActionResult AdvancedSearchGetHitBookIdsPaged(string json, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)//TODO mock, should return array of ids of books where results ocurred
        {
            using (var client = GetRestClient())
            {
                var ids = new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                var ids2 = new List<long> { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
                var ids3 = new List<long> { 21, 22, 23, 24 };
                var totalCount = 3;
                if (start < 10)
                {
                    totalCount = 24;
                    return Json(new { list = ids, totalCount = totalCount });
                }
                if (start >= 10 && start < 20)
                {
                    totalCount = 24;
                    return Json(new { list = ids2, totalCount = totalCount });
                }
                if (start >= 20 && start < 30)
                {
                    totalCount = 24;
                    return Json(new { list = ids3, totalCount = totalCount });
                }
                ids = null;
                totalCount = 24;
                return Json(new { list = ids, totalCount = totalCount });
            }
        }

        public ActionResult GetAllPages(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)//TODO mock
        {
            Thread.Sleep(3000);
            return Json(new
            {
                totalCount = 20,
                pages = new { }
            });
            //return BadRequest();
        }

        public ActionResult AdvancedGetAllPages(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)//TODO mock
        {
            return Json(new {});
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

        public ActionResult TextSearchFulltextGetBookPage(string text, int start, int count, int contextLength, short sortingEnum, bool sortAsc,
            long bookId)//TODO should return page with a certain amount of search results from a selected book id
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text can't be null in fulltext search");
            }

            var selectedBookIds = new List<long> {bookId};//TODO mock
            var notes = new List<string>();
            notes.Add("note1");
            notes.Add("note2");
            notes.Add("note3");

            using (var client = GetRestClient())
            {
                var resultList = new List<CorpusSearchResultContract>();
                if (start<12)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        var result = new CorpusSearchResultContract
                        {
                            Author = "Autor",
                            Title = "Title " + selectedBookIds[0],
                            SourceAbbreviation = "Book" + bookId,
                            RelicAbbreviation = "TIT",
                            BookId = selectedBookIds[0],
                            OriginDate = "first half of 8th century",
                            Notes = notes,
                            PageResultContext = new PageWithContextContract
                            {
                                ContextStructure = new KwicStructure
                                {
                                    Before = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa before ",
                                    Match = "match"+start,
                                    After = " after ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd"
                                }
                            }
                        };
                        resultList.Add(result);
                    }
                }
                if(start>=12 && start<=15)
                {
                    var random = new Random();
                    var randomNumber = random.Next(0, 4);
                    for (var i = 0; i < randomNumber; i++)
                    {
                        var result = new CorpusSearchResultContract
                        {
                            Author = "Autor",
                            Title = "Title " + selectedBookIds[0],
                            SourceAbbreviation = "Book" + bookId,
                            RelicAbbreviation = "TIT",
                            BookId = selectedBookIds[0],
                            OriginDate = "first half of 8th century",
                            Notes = notes,
                            PageResultContext = new PageWithContextContract
                            {
                                ContextStructure = new KwicStructure
                                {
                                    Before = "before ",
                                    Match = "match"+start,
                                    After = " after"
                                }
                            }
                        };
                        resultList.Add(result);
                    }
                }

                return Json(new { results = resultList });
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

        public ActionResult AdvancedSearchCorpusPaged(string json, int start, int count, int contextLength, short sortingEnum, bool sortAsc,
            IList<long> selectedBookIds, IList<int> selectedCategoryIds)
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

        public ActionResult AdvancedSearchCorpusGetPage(string json, int start, int count, int contextLength, short sortingEnum, bool sortAsc,
            long bookId)//TODO should return page with a certain amount of search results from a selected book id, search by json
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            if (listSearchCriteriaContracts.FirstOrDefault(x => x.Key == CriteriaKey.Fulltext || x.Key == CriteriaKey.Heading || x.Key == CriteriaKey.Sentence || x.Key == CriteriaKey.TokenDistance) == null) //TODO add check on string values empty
            {
                throw new ArgumentException("search in text can't be ommited");
            }

            var selectedBookIds = new List<long> {bookId};//TODO mock
            var selectedCategoryIds = new List<int>();//TODO mock

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            using (var client = GetRestClient())
            {
                var resultList = new List<CorpusSearchResultContract>();
                if (start < 12)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        var result = new CorpusSearchResultContract
                        {
                            Author = "Autor",
                            Title = "Title " + selectedBookIds[0],
                            RelicAbbreviation = "TIT",
                            SourceAbbreviation = "Book"+ bookId,
                            BookId = selectedBookIds[0],
                            OriginDate = "first half of 8th century",
                            PageResultContext = new PageWithContextContract
                            {
                                ContextStructure = new KwicStructure
                                {
                                    Before = "before ",
                                    Match = "match",
                                    After = " after"
                                }
                            }
                        };
                        resultList.Add(result);
                    }
                }
                if (start >= 12 && start <= 15)
                {
                    var random = new Random();
                    var randomNumber = random.Next(0, 4);
                    for (var i = 0; i < randomNumber; i++)
                    {
                        var result = new CorpusSearchResultContract
                        {
                            Author = "Autor",
                            Title = "Title " + selectedBookIds[0],
                            RelicAbbreviation = "TIT",
                            SourceAbbreviation = "Book" + bookId,
                            BookId = selectedBookIds[0],
                            OriginDate = "first half of 8th century",
                            PageResultContext = new PageWithContextContract
                            {
                                ContextStructure = new KwicStructure
                                {
                                    Before = "before ",
                                    Match = "match",
                                    After = " after"
                                }
                            }
                        };
                        resultList.Add(result);
                    }
                }

                return Json(new { results = resultList });
            }
        }

        #endregion
    }
}