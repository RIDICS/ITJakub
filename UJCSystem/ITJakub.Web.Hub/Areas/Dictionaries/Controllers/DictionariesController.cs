using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mime;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Areas.Dictionaries.Models;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using ITJakub.Web.Hub.Models.Requests.Dictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Areas.Dictionaries.Controllers
{
    [Area("Dictionaries")]
    public class DictionariesController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;

        public DictionariesController(StaticTextManager staticTextManager, CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
        }

        public override BookTypeEnumContract AreaBookType { get { return BookTypeEnumContract.Dictionary; } }

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
            using (var client = GetMainServiceClient())
            {
                var dictionariesAndCategories = client.GetBooksWithCategoriesByBookType(AreaBookType);

                return Json(dictionariesAndCategories);
            }
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextDictionaryInfo);
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeFeedback);

            var username = HttpContext.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return View(new HeadwordFeedbackViewModel
                {
                    PageStaticText = pageStaticText
                });
            }

            using (var client = GetEncryptedClient())
            {
                var user = client.FindUserByUserName(username);
                var viewModel = new HeadwordFeedbackViewModel
                {
                    Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                    Email = user.Email,
                    PageStaticText = pageStaticText
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

        private bool IsNullOrEmpty<T>(IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public ActionResult SearchBasicResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContractBasic = new List<SearchCriteriaContract>();

            if (!string.IsNullOrEmpty(text))
            {
                searchContractBasic.Add(CreateWordListContract(CriteriaKey.Headword, text));
            }
            if (!IsNullOrEmpty(selectedBookIds) || !IsNullOrEmpty(selectedCategoryIds))
            {
                searchContractBasic.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            using (var client = GetMainServiceClient())
            {
                var headwordCount = client.SearchHeadwordByCriteriaResultsCount(searchContractBasic, DictionarySearchTarget.Headword);
                return Json(headwordCount);
            }
        }

        public ActionResult SearchBasicFulltextResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var searchContractFulltext = new List<SearchCriteriaContract>();

            if (!string.IsNullOrEmpty(text))
            {
                searchContractFulltext.Add(CreateWordListContract(CriteriaKey.HeadwordDescription, text));
            }
            if (!IsNullOrEmpty(selectedBookIds) || !IsNullOrEmpty(selectedCategoryIds))
            {
                searchContractFulltext.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            using (var client = GetMainServiceClient())
            {
                var fulltextCount = client.SearchHeadwordByCriteriaResultsCount(searchContractFulltext, DictionarySearchTarget.Fulltext);
                return Json(fulltextCount);
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
            if (!IsNullOrEmpty(selectedBookIds) || !IsNullOrEmpty(selectedCategoryIds))
            {
                searchContract.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            using (var client = GetMainServiceClient())
            {
                var result = client.SearchHeadwordByCriteria(searchContract, DictionarySearchTarget.Headword);
                return Json(result);
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
            if (!IsNullOrEmpty(selectedBookIds) || !IsNullOrEmpty(selectedCategoryIds))
            {
                searchContract.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            using (var client = GetMainServiceClient())
            {
                var result = client.SearchHeadwordByCriteria(searchContract, DictionarySearchTarget.Fulltext);
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult SearchCriteriaResultsCount([FromBody] DictionarySearchCriteriaCountRequest request)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(request.Json);

            if (!IsNullOrEmpty(request.SelectedBookIds) || !IsNullOrEmpty(request.SelectedCategoryIds))
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(request.SelectedBookIds, request.SelectedCategoryIds));
            }
            using (var client = GetMainServiceClient())
            {
                var resultCount = client.SearchHeadwordByCriteriaResultsCount(listSearchCriteriaContracts, DictionarySearchTarget.Fulltext);
                return Json(resultCount);
            }
        }

        [HttpPost]
        public ActionResult SearchCriteria([FromBody] DictionarySearchCriteriaRequest request)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(request.Json);
            listSearchCriteriaContracts.Add(CreateResultCriteriaContract(request.Start, request.Count));

            if (!IsNullOrEmpty(request.SelectedBookIds) || !IsNullOrEmpty(request.SelectedCategoryIds))
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(request.SelectedBookIds, request.SelectedCategoryIds));
            }
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
            using (var client = GetMainServiceClient())
            {
                var resultCount = client.GetHeadwordCount(selectedCategoryIds, selectedBookIds, AreaBookType);
                return Json(resultCount);
            }
        }

        public ActionResult GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int page, int pageSize)
        {
            using (var client = GetMainServiceClient())
            {
                var start = (page - 1)*pageSize;
                HeadwordListContract result = client.GetHeadwordList(selectedCategoryIds, selectedBookIds, start, pageSize, AreaBookType);
                return Json(result);
            }
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
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadDictionaryHeadwords(selectedCategoryIds, selectedBookIds, query, AreaBookType);
                return Json(result);
            }
        }

        public ActionResult GetTypeaheadTitle(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadTitlesByBookType(query, AreaBookType, selectedCategoryIds, selectedBookIds);
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
        }

        public ActionResult DictionaryAdvancedSearchResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);

            if (!IsNullOrEmpty(selectedBookIds) || !IsNullOrEmpty(selectedCategoryIds))
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }
            using (var client = GetMainServiceClient())
            {
                var count = client.SearchCriteriaResultsCount(listSearchCriteriaContracts);
                return Json(new {count});
            }
        }

        public ActionResult DictionaryAdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);
            listSearchCriteriaContracts.Add(CreateResultCriteriaContract(start, count, sortingEnum, sortAsc));

            if (!IsNullOrEmpty(selectedBookIds) || !IsNullOrEmpty(selectedCategoryIds))
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }
            using (var client = GetMainServiceClient())
            {
                var results = client.SearchByCriteria(listSearchCriteriaContracts);
                return Json(new {books = results}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult DictionaryBasicSearchResultsCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>();

            if (!string.IsNullOrEmpty(text))
            {
                listSearchCriteriaContracts.Add(CreateWordListContract(CriteriaKey.Title, text));
            }
            if (!IsNullOrEmpty(selectedBookIds) || !IsNullOrEmpty(selectedCategoryIds))
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            using (var client = GetMainServiceClient())
            {
                var count = client.SearchCriteriaResultsCount(listSearchCriteriaContracts);
                return Json(new {count});
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
            if (!IsNullOrEmpty(selectedBookIds) || !IsNullOrEmpty(selectedCategoryIds))
            {
                listSearchCriteriaContracts.Add(CreateCategoryCriteriaContract(selectedBookIds, selectedCategoryIds));
            }

            using (var client = GetMainServiceClient())
            {
                var results = client.SearchByCriteria(listSearchCriteriaContracts);
                return Json(new {books = results}, GetJsonSerializerSettingsForBiblModule());
            }
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