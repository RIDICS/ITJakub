﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Web.Hub.Areas.Dictionaries.Models;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using ITJakub.Web.Hub.Models.Requests.Dictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Dictionaries.Controllers
{
    [Area("Dictionaries")]
    public class DictionariesController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public DictionariesController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager, CommunicationProvider communicationProvider) : base(communicationProvider)
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

        public ActionResult Listing(string xmlId, string[] books)
        {
            if (!string.IsNullOrEmpty(xmlId))
            {
                using (var client = GetMainServiceClient())
                {
                    var bookId = client.GetBookIdByXmlId(xmlId);
                    var bookArrId = string.Format("[{0}]", bookId);

                    return RedirectToAction("Listing", "Dictionaries", new { books = bookArrId });
                }
            }

            return View();
        }

        public ActionResult Help()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextDictionaryHelp);
            return View(pageStaticText);
        }

        public ActionResult GetDictionariesWithCategories()
        {
            var result = GetBooksAndCategories();
            return Json(result);
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextDictionaryInfo);
            return View(pageStaticText);
        }

        public ActionResult Feedback(string bookId, string versionId, string entryId, string headword, string dictionary)
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeFeedback);
            var viewModel = new HeadwordFeedbackViewModel
            {
                BookXmlId = bookId,
                BookVersionXmlId = versionId,
                EntryXmlId = entryId,
                Dictionary = dictionary,
                Headword = headword,
                PageStaticText = pageStaticText
            };

            var username = GetUserName();
            if (string.IsNullOrWhiteSpace(username))
            {
                return View(viewModel);
            }

            using (var client = GetEncryptedClient())
            {
                var user = client.FindUserByUserName(username);
                viewModel.Name = string.Format("{0} {1}", user.FirstName, user.LastName);
                viewModel.Email = user.Email;

                return View(viewModel);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(HeadwordFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeFeedback);
                return View(model);
            }

            if (model.BookXmlId == null || model.BookVersionXmlId == null || model.EntryXmlId == null)
            {
                m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.Dictionaries, GetMainServiceClient(), IsUserLoggedIn(), GetUserName());
            }
            else
            {
                AddHeadwordFeedback(model.Text, model.BookXmlId, model.BookVersionXmlId, model.EntryXmlId, model.Name, model.Email);
            }
                
            return View("Feedback/FeedbackSuccess");
        }

        private List<SearchCriteriaContract> DeserializeJsonSearchCriteria(string json)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
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

            using (var client = GetRestClient())
            {
                var newRequest = new HeadwordSearchRequestContract
                {
                    ConditionConjunction = searchContractBasic,
                };
                var resultCount = client.SearchHeadwordCount(newRequest);
                return Json(resultCount);
            }
        }

        public ActionResult SearchBasicFulltextResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContractFulltext = CreateTextCriteriaList(CriteriaKey.HeadwordDescription, text);

            AddCategoryCriteria(searchContractFulltext, selectedBookIds, selectedCategoryIds);
            
            using (var client = GetRestClient())
            {
                var newRequest = new HeadwordSearchRequestContract
                {
                    ConditionConjunction = searchContractFulltext,
                };
                var resultCount = client.SearchHeadwordCount(newRequest);
                return Json(resultCount);
            }
        }

        public ActionResult SearchBasicHeadword(string text, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContractBasic = CreateTextCriteriaList(CriteriaKey.Headword, text);

            AddCategoryCriteria(searchContractBasic, selectedBookIds, selectedCategoryIds);

            var result = SearchHeadwordByCriteria(searchContractBasic, start, count);
            return Json(result);
        }

        public ActionResult SearchBasicFulltext(string text, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
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

            using (var client = GetRestClient())
            {
                var newRequest = new HeadwordSearchRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts,
                };
                var resultCount = client.SearchHeadwordCount(newRequest);
                return Json(resultCount);
            }
        }

        [HttpPost]
        public ActionResult SearchCriteria([FromBody] DictionarySearchCriteriaRequest request)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(request.Json);
            
            AddCategoryCriteria(listSearchCriteriaContracts, request.SelectedBookIds, request.SelectedCategoryIds);

            var result = SearchHeadwordByCriteria(listSearchCriteriaContracts, request.Start, request.Count);
            return Json(result);
        }

        private HeadwordListContract SearchHeadwordByCriteria(List<SearchCriteriaContract> listSearchCriteriaContracts,
            int start, int count)
        {
            using (var client = GetRestClient())
            {
                // Search headwords
                var newRequest = new HeadwordSearchRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts,
                    Start = start,
                    Count = count,
                };
                var resultHeadwords = client.SearchHeadword(newRequest);

                // Load info about dictionaries/books
                var bookListDictionary = new Dictionary<string, DictionaryContract>();
                foreach (var projectId in resultHeadwords.Select(x => x.ProjectId).Distinct())
                {
                    var bookInfo = client.GetProjectMetadata(projectId, false, false, false, false, false);
                    var resultBookInfo = new DictionaryContract
                    {
                        BookId = projectId,
                        BookXmlId = projectId.ToString(), // TODO remove
                        BookAcronym = bookInfo.SourceAbbreviation,
                        BookTitle = bookInfo.Title,
                        BookVersionId = 0, // TODO not specified
                        BookVersionXmlId = null //TODO not specified
                    };
                    bookListDictionary.Add(projectId.ToString(), resultBookInfo);
                }

                // Create response
                var result = new HeadwordListContract
                {
                    HeadwordList = MapHeadwordsToGroupedList(resultHeadwords),
                    BookList = bookListDictionary
                };

                return result;
            }
        }

        private List<HeadwordContract> MapHeadwordsToGroupedList(List<Vokabular.MainService.DataContracts.Contracts.HeadwordContract> headwords)
        {
            var resultList = new List<HeadwordContract>();
            HeadwordContract lastHeadword = null;
            foreach (var headwordContract in headwords)
            {
                if (lastHeadword == null || lastHeadword.Headword != headwordContract.DefaultHeadword)
                {
                    lastHeadword = new HeadwordContract
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
                        BookXmlId = headwordContract.ProjectId.ToString(), // TODO change XmlId to normal ID
                        EntryXmlId = headwordContract.Id.ToString(),
                        Image = null, //TODO remove Image
                        PageId = resourcePageId, // TODO add PageId usage to DictionaryViewer
                    };
                    lastHeadword.Dictionaries.Add(dictionaryInfo);
                }
            }

            return resultList;
        }

        public ActionResult GetHeadwordDescription(string bookGuid, string xmlEntryId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetDictionaryEntryByXmlId(bookGuid, xmlEntryId, OutputFormatEnumContract.Html, AreaBookType);
                return Json(result);
            }
        }

        public ActionResult GetHeadwordDescriptionFromSearch(string criteria, string bookGuid, string xmlEntryId, bool isCriteriaJson)
        {
            IList<SearchCriteriaContract> listSearchCriteriaContracts;
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
            using (var client = GetMainServiceClient())
            {
                var result = client.GetDictionaryEntryFromSearch(listSearchCriteriaContracts, bookGuid, xmlEntryId, OutputFormatEnumContract.Html,
                    AreaBookType);
                return Json(result);
            }
        }

        public ActionResult GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>();
            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            using (var client = GetRestClient())
            {
                var newRequest = new HeadwordSearchRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts,
                };
                var resultCount = client.SearchHeadwordCount(newRequest);
                return Json(resultCount);
            }
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
            using (var client = GetMainServiceClient())
            {
                var rowNumber = client.GetHeadwordRowNumber(selectedCategoryIds, selectedBookIds, query, AreaBookType);

                var resultPageNumber = (rowNumber - 1) / pageSize + 1;

                return Json(resultPageNumber);
            }
        }

        public ActionResult GetHeadwordPageNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string headwordBookId,
            string headwordEntryXmlId, int pageSize)
        {
            using (var client = GetMainServiceClient())
            {
                var rowNumber = client.GetHeadwordRowNumberById(selectedCategoryIds, selectedBookIds, headwordBookId, headwordEntryXmlId, AreaBookType);
                var resultPageNumber = (rowNumber - 1)/pageSize + 1;
                return Json(resultPageNumber);
            }
        }

        public ActionResult GetTypeaheadDictionaryHeadword(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetHeadwordAutocomplete(query, AreaBookType, selectedCategoryIds, selectedBookIds);
                return Json(result);
            }
        }
        
        public ActionResult GetHeadwordBookmarks()
        {
            using (var client = GetMainServiceClient())
            {
                var list = client.GetHeadwordBookmarks();
                return Json(list);
            }
        }
        
        public ActionResult AddHeadwordBookmark([FromBody] AddHeadwordBookmarkRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.AddHeadwordBookmark(request.BookId, request.EntryXmlId);
                return Json(new {});
            }
        }

        public ActionResult RemoveHeadwordBookmark([FromBody] RemoveHeadwordBookmarkRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.RemoveHeadwordBookmark(request.BookId, request.EntryXmlId);
                return Json(new {});
            }
        }

        public FileResult GetHeadwordImage(string bookXmlId, string bookVersionXmlId, string fileName)
        {
            using (var client = GetMainServiceClient())
            {
                var resultStream = client.GetHeadwordImage(bookXmlId, bookVersionXmlId, fileName);
                return File(resultStream, MediaTypeNames.Image.Jpeg); //TODO resolve content type properly
            }
        }

        private void AddHeadwordFeedback(string content, string bookXmlId, string bookVersionXmlId, string entryXmlId, string name, string email)
        {
            var username = HttpContext.User.Identity.Name;
            using (var client = GetMainServiceClient())
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    client.CreateAnonymousFeedbackForHeadword(content, bookXmlId, bookVersionXmlId, entryXmlId, name, email);
                }
                else
                {
                    client.CreateFeedbackForHeadword(content, bookXmlId, bookVersionXmlId, entryXmlId, username);
                }
            }
        }

        public ActionResult DictionaryAdvancedSearchResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaJsonCount(json, selectedBookIds, selectedCategoryIds);
            return Json(new { count });
        }

        public ActionResult DictionaryAdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaJson(json, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = result }, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult DictionaryBasicSearchResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Title, text, selectedBookIds, selectedCategoryIds);
            return Json(new { count });
        }

        public ActionResult DictionaryBasicSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var results = SearchByCriteriaText(CriteriaKey.Title, text, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = results }, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult GetDictionaryInfo(string bookXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetBookInfoWithPages(bookXmlId);
                return Json(result);
            }
        }
    }
}