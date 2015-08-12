﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mime;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Areas.Dictionaries.Models;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Areas.Dictionaries.Controllers
{
    [RouteArea("Dictionaries")]
    public class DictionariesController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient;
        private readonly ItJakubServiceEncryptedClient m_mainServiceEncryptedClient;

        public DictionariesController()
        {
            m_mainServiceClient = new ItJakubServiceClient();
            m_mainServiceEncryptedClient = new ItJakubServiceEncryptedClient();
        }

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

        public ActionResult Listing()
        {
            return View();
        }
        public ActionResult Help()
        {
            return View();
        }

        public ActionResult GetDictionariesWithCategories()
        {
            var dictionariesAndCategories =
                m_mainServiceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Dictionary);

            return Json(dictionariesAndCategories, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult FeedBack()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(HeadwordFeedbackViewModel model)
        {
            AddHeadwordFeedback(model.Text, model.BookXmlId, model.BookVersionXmlId, model.EntryXmlId, model.Name, model.Email);
            return View("Information");
        }

        private IList<SearchCriteriaContract> DeserializeJsonSearchCriteria(string json)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            return Mapper.Map<IList<SearchCriteriaContract>>(deserialized);
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

        private ResultCriteriaContract CreateResultCriteriaContract(int start, int count)
        {
            return new ResultCriteriaContract
            {
                Start = start,
                Count = count
            };
        }

        private ResultCriteriaContract CreateResultCriteriaContract(int start, int count, short sortingEnum, bool sortAsc)
        {
            return new ResultCriteriaContract
            {
                Start = start,
                Count = count,
                Sorting = (SortEnum) sortingEnum,
                Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending,
            };
        }

        private SelectedCategoryCriteriaContract CreateCategoryCriteriaContract(IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            return new SelectedCategoryCriteriaContract
            {
                SelectedBookIds = selectedBookIds,
                SelectedCategoryIds = selectedCategoryIds
            };
        }

        public ActionResult SearchBasicResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContractBasic = new List<SearchCriteriaContract>
            {
                CreateWordListContract(CriteriaKey.Headword, text),
            };
            var searchContractFulltext = new List<SearchCriteriaContract>
            {
                CreateWordListContract(CriteriaKey.HeadwordDescription, text),
            };

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                searchContractBasic.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
                searchContractFulltext.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }
            
            var headwordCount = m_mainServiceClient.SearchHeadwordByCriteriaResultsCount(searchContractBasic, DictionarySearchTarget.Headword);
            var fulltextCount = m_mainServiceClient.SearchHeadwordByCriteriaResultsCount(searchContractFulltext, DictionarySearchTarget.Fulltext);
            
            var result = new HeadwordSearchResultContract
            {
                HeadwordCount = headwordCount,
                FulltextCount = fulltextCount
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchBasicHeadword(string text, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContract = new List<SearchCriteriaContract>
            {
                CreateWordListContract(CriteriaKey.Headword, text),
                CreateResultCriteriaContract(start, count),
            };

            if (selectedBookIds != null || selectedCategoryIds != null)
                searchContract.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));

            var result = m_mainServiceClient.SearchHeadwordByCriteria(searchContract, DictionarySearchTarget.Headword);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchBasicFulltext(string text, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContract = new List<SearchCriteriaContract>
            {
                CreateWordListContract(CriteriaKey.HeadwordDescription, text),
                CreateResultCriteriaContract(start, count),
            };

            if (selectedBookIds != null || selectedCategoryIds != null)
                searchContract.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));

            var result = m_mainServiceClient.SearchHeadwordByCriteria(searchContract, DictionarySearchTarget.Fulltext);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SearchCriteriaResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);

            if (selectedBookIds != null || selectedCategoryIds != null)
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));

            var resultCount = m_mainServiceClient.SearchHeadwordByCriteriaResultsCount(listSearchCriteriaContracts, DictionarySearchTarget.Fulltext);
            return Json(resultCount);
        }

        [HttpPost]
        public ActionResult SearchCriteria(string json, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);
            listSearchCriteriaContracts.Add(CreateResultCriteriaContract(start, count));

            if (selectedBookIds != null || selectedCategoryIds != null)
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            
            var result = m_mainServiceClient.SearchHeadwordByCriteria(listSearchCriteriaContracts, DictionarySearchTarget.Fulltext);
            return Json(result);
        }
        
        public ActionResult GetHeadwordDescription(string bookGuid, string xmlEntryId)
        {
            var result = m_mainServiceClient.GetDictionaryEntryByXmlId(bookGuid, xmlEntryId, OutputFormatEnumContract.Html);
            return Json(result, JsonRequestBehavior.AllowGet);
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

            var result = m_mainServiceClient.GetDictionaryEntryFromSearch(listSearchCriteriaContracts, bookGuid, xmlEntryId, OutputFormatEnumContract.Html);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            var resultCount = m_mainServiceClient.GetHeadwordCount(selectedCategoryIds, selectedBookIds);
            return Json(resultCount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int page, int pageSize)
        {
            var start = (page - 1)*pageSize;
            var result = m_mainServiceClient.GetHeadwordList(selectedCategoryIds, selectedBookIds, start, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadwordPageNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query, int pageSize)
        {
            var rowNumber = m_mainServiceClient.GetHeadwordRowNumber(selectedCategoryIds, selectedBookIds, query);
            var resultPageNumber = (rowNumber - 1)/pageSize + 1;
            return Json(resultPageNumber, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadwordPageNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string headwordBookId, string headwordEntryXmlId, int pageSize)
        {
            var rowNumber = m_mainServiceClient.GetHeadwordRowNumberById(selectedCategoryIds, selectedBookIds, headwordBookId, headwordEntryXmlId);
            var resultPageNumber = (rowNumber - 1) / pageSize + 1;
            return Json(resultPageNumber, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadDictionaryHeadword(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var result = m_mainServiceClient.GetTypeaheadDictionaryHeadwords(selectedCategoryIds, selectedBookIds, query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadTitle(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var result = m_mainServiceClient.GetTypeaheadTitlesByBookType(query, BookTypeEnumContract.Dictionary, selectedCategoryIds, selectedBookIds);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadwordBookmarks()
        {
            var list = m_mainServiceEncryptedClient.GetHeadwordBookmarks(HttpContext.User.Identity.Name);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddHeadwordBookmark(string bookId, string entryXmlId)
        {
            m_mainServiceEncryptedClient.AddHeadwordBookmark(bookId, entryXmlId, HttpContext.User.Identity.Name);
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveHeadwordBookmark(string bookId, string entryXmlId)
        {
            m_mainServiceEncryptedClient.RemoveHeadwordBookmark(bookId, entryXmlId, HttpContext.User.Identity.Name);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public FileResult GetHeadwordImage(string bookXmlId, string bookVersionXmlId, string fileName)
        {
            var resultStream = m_mainServiceClient.GetHeadwordImage(bookXmlId, bookVersionXmlId, fileName);
            return File(resultStream, MediaTypeNames.Image.Jpeg); //TODO resolve content type properly
        }

        public ActionResult AddHeadwordFeedback(string content, string bookXmlId, string bookVersionXmlId, string entryXmlId, string name, string email)
        {
            var username = HttpContext.User.Identity.Name;
            if (bookXmlId == null || bookVersionXmlId == null || entryXmlId == null)
            {
                if (string.IsNullOrWhiteSpace(username))
                    m_mainServiceClient.CreateAnonymousFeedback(content, name, email);
                else
                    m_mainServiceEncryptedClient.CreateFeedback(content, username);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(username))
                    m_mainServiceClient.CreateAnonymousFeedbackForHeadword(content, bookXmlId, bookVersionXmlId, entryXmlId, name, email);
                else
                    m_mainServiceEncryptedClient.CreateFeedbackForHeadword(content, bookXmlId, bookVersionXmlId, entryXmlId, username);
            }
            
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DictionaryAdvancedSearchResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            var count = m_mainServiceClient.SearchCriteriaResultsCount(listSearchCriteriaContracts);
            return Json(new { count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DictionaryAdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);
            listSearchCriteriaContracts.Add(CreateResultCriteriaContract(start, count, sortingEnum, sortAsc));

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            var results = m_mainServiceClient.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new { books = results }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DictionaryBasicSearchResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                CreateWordListContract(CriteriaKey.Title, text)
            };

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            var count = m_mainServiceClient.SearchCriteriaResultsCount(listSearchCriteriaContracts);
            return Json(new { count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DictionaryBasicSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                CreateWordListContract(CriteriaKey.Title, text),
                CreateResultCriteriaContract(start, count, sortingEnum, sortAsc)
            };

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            var results = m_mainServiceClient.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new { books = results }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDictionaryInfo(string bookXmlId)
        {
            var result = m_mainServiceClient.GetBookInfo(bookXmlId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}