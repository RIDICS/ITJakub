using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.Web.Hub.Areas.Dictionaries.Models;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.DataContracts;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using ITJakub.Web.Hub.Models.Requests.Dictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.AspNetCore.Extensions;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;
using ITJakub.Web.Hub.Options;

namespace ITJakub.Web.Hub.Areas.Dictionaries.Controllers
{
    [LimitedAccess(PortalType.ResearchPortal)]
    [Area("Dictionaries")]
    public class DictionariesController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public DictionariesController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager,
            CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.Dictionary;

        public ActionResult Index()
        {
            return View("Search");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Listing(string xmlId, string externalId) // string[] books - this parameter is used in JavaScript
        {
            // xmlId paramater is for already existing hyperlinks, new parameter is externalId
            externalId = externalId ?? xmlId;

            if (!string.IsNullOrEmpty(externalId)) // request to one specific book using externalId
            {
                var client = GetBookClient();
                var book = client.GetBookInfoByExternalId(externalId);
                var bookArrId = $"[{book.Id}]";

                return RedirectToAction("Listing", "Dictionaries", new {books = bookArrId});
            }

            return View();
        }

        public ActionResult Help()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextDictionaryHelp, "dict");
            return View(pageStaticText);
        }

        public ActionResult GetDictionariesWithCategories()
        {
            var result = GetBooksAndCategories();
            return Json(result);
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextDictionaryInfo, "dict");
            return View(pageStaticText);
        }

        public ActionResult Feedback(long? bookId, long? headwordVersionId, string headword, string dictionary)
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeFeedback, "home");
            var viewModel = new HeadwordFeedbackViewModel
            {
                BookId = bookId,
                HeadwordVersionId = headwordVersionId,
                Dictionary = dictionary,
                Headword = headword,
                PageStaticText = pageStaticText
            };

            if (!IsUserLoggedIn())
            {
                return View(viewModel);
            }

            viewModel.Name = $"{User.GetFirstName()} {User.GetLastName()}";
            viewModel.Email = User.GetEmail();

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(HeadwordFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeFeedback, "home");
                return View(model);
            }

            if (model.BookId == null || model.HeadwordVersionId == null)
            {
                m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.Dictionaries, IsUserLoggedIn());
            }
            else
            {
                AddHeadwordFeedback(model.Text, model.HeadwordVersionId.Value, model.Name, model.Email);
            }

            return View("Feedback/FeedbackSuccess");
        }

        private List<SearchCriteriaContract> DeserializeJsonSearchCriteria(string json)
        {
            var deserialized =
                JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            return Mapper.Map<List<SearchCriteriaContract>>(deserialized);
        }

        private WordListCriteriaContract CreateWordListContract(CriteriaKey criteriaKey, string text)
        {
            return new WordListCriteriaContract
            {
                Key = criteriaKey,
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract {Contains = new List<string> {text}}
                }
            };
        }

        public ActionResult SearchBasicResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContractBasic = CreateTextCriteriaList(CriteriaKey.Headword, text);

            AddCategoryCriteria(searchContractBasic, selectedBookIds, selectedCategoryIds);


            var newRequest = new HeadwordSearchRequestContract
            {
                ConditionConjunction = searchContractBasic,
            };
            var client = GetBookClient();
            var resultCount = client.SearchHeadwordCount(newRequest);
            return Json(resultCount);
        }

        public ActionResult SearchBasicFulltextResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContractFulltext = CreateTextCriteriaList(CriteriaKey.HeadwordDescription, text);

            AddCategoryCriteria(searchContractFulltext, selectedBookIds, selectedCategoryIds);

            var newRequest = new HeadwordSearchRequestContract
            {
                ConditionConjunction = searchContractFulltext,
            };
            var client = GetBookClient();
            var resultCount = client.SearchHeadwordCount(newRequest);
            return Json(resultCount);
        }

        public ActionResult SearchBasicHeadword(string text, int start, int count, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var searchContractBasic = CreateTextCriteriaList(CriteriaKey.Headword, text);

            AddCategoryCriteria(searchContractBasic, selectedBookIds, selectedCategoryIds);

            var result = SearchHeadwordByCriteria(searchContractBasic, start, count);
            return Json(result);
        }

        public ActionResult SearchBasicFulltext(string text, int start, int count, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var searchContractFulltext = CreateTextCriteriaList(CriteriaKey.HeadwordDescription, text);

            AddCategoryCriteria(searchContractFulltext, selectedBookIds, selectedCategoryIds);

            var result = SearchHeadwordByCriteria(searchContractFulltext, start, count);
            return Json(result);
        }

        [HttpPost]
        public ActionResult SearchCriteriaResultsCount([FromBody] DictionarySearchCriteriaCountRequest request)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(request.Json);

            AddCategoryCriteria(listSearchCriteriaContracts, request.SelectedBookIds, request.SelectedCategoryIds);

            var newRequest = new HeadwordSearchRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts,
            };
            var client = GetBookClient();
            var resultCount = client.SearchHeadwordCount(newRequest);
            return Json(resultCount);
        }

        [HttpPost]
        public ActionResult SearchCriteria([FromBody] DictionarySearchCriteriaRequest request)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(request.Json);

            AddCategoryCriteria(listSearchCriteriaContracts, request.SelectedBookIds, request.SelectedCategoryIds);

            var result = SearchHeadwordByCriteria(listSearchCriteriaContracts, request.Start, request.Count);
            return Json(result);
        }

        private ResultHeadwordListContract SearchHeadwordByCriteria(List<SearchCriteriaContract> listSearchCriteriaContracts,
            int start, int count)
        {
            var client = GetBookClient();
            // Search headwords
            var newRequest = new HeadwordSearchRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts,
                Start = start,
                Count = count,
            };
            var resultHeadwords = client.SearchHeadword(newRequest);

            // Load info about dictionaries/books
            var bookListDictionary = new Dictionary<long, BookContract>();
            foreach (var projectId in resultHeadwords.Select(x => x.ProjectId).Distinct())
            {
                var bookInfo = client.GetBookInfo(projectId);
                bookListDictionary.Add(projectId, bookInfo);
            }

            // Create response
            var result = new ResultHeadwordListContract
            {
                HeadwordList = MapHeadwordsToGroupedList(resultHeadwords),
                BookList = bookListDictionary
            };

            return result;
        }

        private List<HeadwordWithDictionariesContract> MapHeadwordsToGroupedList(List<HeadwordContract> headwords)
        {
            var resultList = new List<HeadwordWithDictionariesContract>();
            HeadwordWithDictionariesContract lastHeadword = null;
            foreach (var headwordContract in headwords)
            {
                if (lastHeadword == null || lastHeadword.Headword != headwordContract.DefaultHeadword)
                {
                    lastHeadword = new HeadwordWithDictionariesContract
                    {
                        Headword = headwordContract.DefaultHeadword,
                        Dictionaries = new List<HeadwordBookInfoContract>()
                    };
                    resultList.Add(lastHeadword);
                }

                foreach (var resourcePageId in headwordContract.HeadwordItems.Select(x => x.ResourcePageId).Distinct())
                {
                    var dictionaryInfo = new HeadwordBookInfoContract
                    {
                        BookId = headwordContract.ProjectId,
                        HeadwordId = headwordContract.Id,
                        HeadwordVersionId = headwordContract.VersionId,
                        PageId = resourcePageId,
                    };
                    lastHeadword.Dictionaries.Add(dictionaryInfo);
                }
            }

            return resultList;
        }

        public ActionResult GetHeadwordDescription(long headwordId)
        {
            var client = GetBookClient();
            var result = client.GetHeadwordText(headwordId, TextFormatEnumContract.Html);
            return Json(result);
        }

        public ActionResult GetHeadwordDescriptionFromSearch(string criteria, bool isCriteriaJson, long headwordId)
        {
            List<SearchCriteriaContract> listSearchCriteriaContracts;
            if (isCriteriaJson)
            {
                listSearchCriteriaContracts = DeserializeJsonSearchCriteria(criteria);
            }
            else
            {
                listSearchCriteriaContracts = new List<SearchCriteriaContract>
                {
                    CreateWordListContract(CriteriaKey.HeadwordDescription, criteria)
                };
            }

            var client = GetBookClient();
            var request = new SearchPageRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            };
            var result = client.GetHeadwordTextFromSearch(headwordId, TextFormatEnumContract.Html, request);
            return Json(result);
        }

        public ActionResult GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>();
            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            var newRequest = new HeadwordSearchRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts,
            };

            var client = GetBookClient();
            var resultCount = client.SearchHeadwordCount(newRequest);
            return Json(resultCount);
        }

        public ActionResult GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int page, int pageSize)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>();
            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            var start = (page - 1) * pageSize;
            var result = SearchHeadwordByCriteria(listSearchCriteriaContracts, start, pageSize);
            return Json(result);
        }

        public ActionResult GetHeadwordPageNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query, int pageSize)
        {
            var request = new HeadwordRowNumberSearchRequestContract
            {
                Query = query,
                Category = new SelectedCategoryCriteriaContract
                {
                    BookType = AreaBookType,
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds,
                }
            };
            var client = GetBookClient();
            var rowNumber = client.SearchHeadwordRowNumber(request);

            var resultPageNumber = (rowNumber - 1) / pageSize + 1;

            return Json(resultPageNumber);
        }

        // Favorite headwords are not currently supported

        //public ActionResult GetHeadwordPageNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string headwordBookId,
        //    string headwordEntryXmlId, int pageSize)
        //{
        //    using (var client = GetMainServiceClient())
        //    {
        //        var rowNumber = client.GetHeadwordRowNumberById(selectedCategoryIds, selectedBookIds, headwordBookId, headwordEntryXmlId, AreaBookType);
        //        var resultPageNumber = (rowNumber - 1)/pageSize + 1;
        //        return Json(resultPageNumber);
        //    }
        //}

        //public ActionResult GetHeadwordBookmarks()
        //{
        //    using (var client = GetMainServiceClient())
        //    {
        //        var list = client.GetHeadwordBookmarks();
        //        return Json(list);
        //    }
        //}

        //public ActionResult AddHeadwordBookmark([FromBody] AddHeadwordBookmarkRequest request)
        //{
        //    using (var client = GetMainServiceClient())
        //    {
        //        client.AddHeadwordBookmark(request.BookId, request.EntryXmlId);
        //        return Json(new {});
        //    }
        //}

        //public ActionResult RemoveHeadwordBookmark([FromBody] RemoveHeadwordBookmarkRequest request)
        //{
        //    using (var client = GetMainServiceClient())
        //    {
        //        client.RemoveHeadwordBookmark(request.BookId, request.EntryXmlId);
        //        return Json(new {});
        //    }
        //}

        public ActionResult GetTypeaheadDictionaryHeadword(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetHeadwordAutocomplete(query, AreaBookType, selectedCategoryIds, selectedBookIds);
                return Json(result);
            }
        }

        //public FileResult GetHeadwordImage(string bookXmlId, string bookVersionXmlId, string fileName) // Original signature
        public ActionResult GetHeadwordImage(long pageId)
        {
            return NotFound();
            //using (var client = GetMainServiceClient())
            //{
            //    var resultStream = client.GetHeadwordImage(bookXmlId, bookVersionXmlId, fileName);
            //    return File(resultStream, MediaTypeNames.Image.Jpeg); //TODO resolve content type properly
            //}
        }

        private void AddHeadwordFeedback(string content, long headwordVersionId, string name, string email)
        {
            var client = GetFeedbackClient();

            if (IsUserLoggedIn())
            {
                client.CreateHeadwordFeedback(headwordVersionId, new CreateFeedbackContract
                {
                    FeedbackCategory = FeedbackCategoryEnumContract.Dictionaries,
                    Text = content,
                });
            }
            else
            {
                client.CreateAnonymousHeadwordFeedback(headwordVersionId, new CreateAnonymousFeedbackContract
                {
                    FeedbackCategory = FeedbackCategoryEnumContract.Dictionaries,
                    Text = content,
                    AuthorEmail = email,
                    AuthorName = name,
                });
            }
        }

        public ActionResult DictionaryAdvancedSearchResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaJsonCount(json, selectedBookIds, selectedCategoryIds);
            return Json(new {count});
        }

        public ActionResult DictionaryAdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc,
            IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaJson(json, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new {books = result}, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult DictionaryBasicSearchResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Title, text, selectedBookIds, selectedCategoryIds);
            return Json(new {count});
        }

        public ActionResult DictionaryBasicSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc,
            IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var results = SearchByCriteriaText(CriteriaKey.Title, text, start, count, sortingEnum, sortAsc, selectedBookIds,
                selectedCategoryIds);
            return Json(new {books = results}, GetJsonSerializerSettingsForBiblModule());
        }
    }
}