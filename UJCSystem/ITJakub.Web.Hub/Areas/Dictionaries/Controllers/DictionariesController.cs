using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mime;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Areas.Dictionaries.Models;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Areas.Dictionaries.Controllers
{
    [RouteArea("Dictionaries")]
    public class DictionariesController : BaseController
    {
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
            using (var client = GetMainServiceClient())
            {
                var dictionariesAndCategories = client.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Dictionary);


                return Json(dictionariesAndCategories, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            var username = HttpContext.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return View();
            }

            using (var client = GetEncryptedClient())
            {
                var user = client.FindUserByUserName(username);
                var viewModel = new HeadwordFeedbackViewModel
                {
                    Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                    Email = user.Email
                };


                return View(viewModel);
            }
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
                Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending
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
            var searchContractBasic = new List<SearchCriteriaContract>();

            if (!string.IsNullOrEmpty(text))
            {
                searchContractBasic.Add(CreateWordListContract(CriteriaKey.Headword, text));
            }

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                searchContractBasic.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            using (var client = GetMainServiceClient())
            {

                var headwordCount = client.SearchHeadwordByCriteriaResultsCount(searchContractBasic, DictionarySearchTarget.Headword);
                return Json(headwordCount, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SearchBasicFulltextResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContractFulltext = new List<SearchCriteriaContract>();

            if (!string.IsNullOrEmpty(text))
            {
                searchContractFulltext.Add(CreateWordListContract(CriteriaKey.HeadwordDescription, text));
            }

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                searchContractFulltext.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }
            using (var client = GetMainServiceClient())
            {
                var fulltextCount = client.SearchHeadwordByCriteriaResultsCount(searchContractFulltext, DictionarySearchTarget.Fulltext);
                return Json(fulltextCount, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SearchBasicHeadword(string text, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContract = new List<SearchCriteriaContract>
            {
                CreateResultCriteriaContract(start, count)
            };

            if (!string.IsNullOrEmpty(text))
            {
                searchContract.Add(CreateWordListContract(CriteriaKey.Headword, text));
            }

            if (selectedBookIds != null || selectedCategoryIds != null)
                searchContract.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchHeadwordByCriteria(searchContract, DictionarySearchTarget.Headword);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SearchBasicFulltext(string text, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContract = new List<SearchCriteriaContract>
            {
                CreateResultCriteriaContract(start, count)
            };

            if (!string.IsNullOrEmpty(text))
            {
                searchContract.Add(CreateWordListContract(CriteriaKey.HeadwordDescription, text));
            }

            if (selectedBookIds != null || selectedCategoryIds != null)
                searchContract.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchHeadwordByCriteria(searchContract, DictionarySearchTarget.Fulltext);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SearchCriteriaResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);

            if (selectedBookIds != null || selectedCategoryIds != null)
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            using (var client = GetMainServiceClient())
            {
                var resultCount = client.SearchHeadwordByCriteriaResultsCount(listSearchCriteriaContracts, DictionarySearchTarget.Fulltext);
                return Json(resultCount);
            }
        }

        [HttpPost]
        public ActionResult SearchCriteria(string json, int start, int count, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);
            listSearchCriteriaContracts.Add(CreateResultCriteriaContract(start, count));

            if (selectedBookIds != null || selectedCategoryIds != null)
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchHeadwordByCriteria(listSearchCriteriaContracts, DictionarySearchTarget.Fulltext);
                return Json(result);
            }
        }

        public ActionResult GetHeadwordDescription(string bookGuid, string xmlEntryId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetDictionaryEntryByXmlId(bookGuid, xmlEntryId, OutputFormatEnumContract.Html, BookTypeEnumContract.Dictionary);
                return Json(result, JsonRequestBehavior.AllowGet);
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
                    BookTypeEnumContract.Dictionary);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            using (var client = GetMainServiceClient())
            {
                var resultCount = client.GetHeadwordCount(selectedCategoryIds, selectedBookIds);
            return Json(resultCount, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int page, int pageSize)
        {
            using (var client = GetMainServiceClient())
            {
                var start = (page - 1)*pageSize;
            HeadwordListContract result = client.GetHeadwordList(selectedCategoryIds, selectedBookIds, start, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult GetHeadwordPageNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query, int pageSize)
        {
            using(var client = GetMainServiceClient()) { 
            var rowNumber = client.GetHeadwordRowNumber(selectedCategoryIds, selectedBookIds, query);
            
            var resultPageNumber = (rowNumber - 1)/pageSize + 1;
            
            return Json(resultPageNumber, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult GetHeadwordPageNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string headwordBookId,
            string headwordEntryXmlId, int pageSize)
        {
            using (var client = GetMainServiceClient())
            {
                var rowNumber = client.GetHeadwordRowNumberById(selectedCategoryIds, selectedBookIds, headwordBookId, headwordEntryXmlId);
            var resultPageNumber = (rowNumber - 1)/pageSize + 1;
            return Json(resultPageNumber, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult GetTypeaheadDictionaryHeadword(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadDictionaryHeadwords(selectedCategoryIds, selectedBookIds, query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult GetTypeaheadTitle(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadTitlesByBookType(query, BookTypeEnumContract.Dictionary, selectedCategoryIds, selectedBookIds);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult GetHeadwordBookmarks()
        {
            using (var client = GetMainServiceClient())
            {
                var list = client.GetHeadwordBookmarks(HttpContext.User.Identity.Name);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        }


        public ActionResult AddHeadwordBookmark(string bookId, string entryXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                client.AddHeadwordBookmark(bookId, entryXmlId, HttpContext.User.Identity.Name);
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult RemoveHeadwordBookmark(string bookId, string entryXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                client.RemoveHeadwordBookmark(bookId, entryXmlId, HttpContext.User.Identity.Name);
            return Json(new {}, JsonRequestBehavior.AllowGet);
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

        public ActionResult AddHeadwordFeedback(string content, string bookXmlId, string bookVersionXmlId, string entryXmlId, string name, string email)
        {
            var username = HttpContext.User.Identity.Name;
            if (bookXmlId == null || bookVersionXmlId == null || entryXmlId == null)
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    using (var client = GetMainServiceClient())
                    {
                        client.CreateAnonymousFeedback(content, name, email, FeedbackCategoryEnumContract.Dictionaries);
                    }
                }

                else
                {
                    using (var client = GetMainServiceClient())
                    {
                        client.CreateFeedback(content, username, FeedbackCategoryEnumContract.Dictionaries);
                    }
                }

            }
            else
            {
                if (string.IsNullOrWhiteSpace(username))
                    using (var client = GetMainServiceClient())
                    {
                        client.CreateAnonymousFeedbackForHeadword(content, bookXmlId, bookVersionXmlId, entryXmlId, name, email);
                    }
                else
                    using (var client = GetMainServiceClient())
                    {
                        client.CreateFeedbackForHeadword(content, bookXmlId, bookVersionXmlId, entryXmlId, username);
                    }
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
            using (var client = GetMainServiceClient())
            {
                var count = client.SearchCriteriaResultsCount(listSearchCriteriaContracts);
            return Json(new {count}, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult DictionaryAdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);
            listSearchCriteriaContracts.Add(CreateResultCriteriaContract(start, count, sortingEnum, sortAsc));

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }
            using (var client = GetMainServiceClient())
            {
                var results = client.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new {books = results}, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult DictionaryBasicSearchResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>();

            if (!string.IsNullOrEmpty(text))
            {
                listSearchCriteriaContracts.Add(CreateWordListContract(CriteriaKey.Title, text));
            }

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }
            using (var client = GetMainServiceClient())
            {
                var count = client.SearchCriteriaResultsCount(listSearchCriteriaContracts);
            return Json(new {count}, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult DictionaryBasicSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                CreateResultCriteriaContract(start, count, sortingEnum, sortAsc)
            };

            if (!string.IsNullOrEmpty(text))
            {
                listSearchCriteriaContracts.Add(CreateWordListContract(CriteriaKey.Title, text));
            }

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }
            using (var client = GetMainServiceClient())
            {
                var results = client.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new {books = results}, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult GetDictionaryInfo(string bookXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetBookInfoWithPages(bookXmlId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        }
    }
}