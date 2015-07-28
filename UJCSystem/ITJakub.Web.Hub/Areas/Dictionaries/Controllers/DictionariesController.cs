using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
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

        // GET: Dictionaries/Dictionaries
        public ActionResult Index()
        {
            return View();
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

        public ActionResult TermsOfUse()
        {
            return View();
        }

        public ActionResult FeedBack()
        {
            return View();
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
            var start = (page - 1)*pageSize + 1;
            var end = page*pageSize;
            var result = m_mainServiceClient.GetHeadwordList(selectedCategoryIds, selectedBookIds, start, end);
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
    }
}