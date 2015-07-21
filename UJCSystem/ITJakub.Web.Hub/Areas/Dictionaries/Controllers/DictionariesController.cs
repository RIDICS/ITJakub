using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
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

        public DictionariesController()
        {
            m_mainServiceClient = new ItJakubServiceClient();
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

        public ActionResult Headwords()
        {
            return View();
        }

        public ActionResult GetTextWithCategories()
        {
            var dictionariesAndCategories =
                m_mainServiceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Edition);
            //var booksDictionary =
            //    dictionariesAndCategories.Books.GroupBy(x => x.CategoryId)
            //        .ToDictionary(x => x.Key.ToString(), x => x.ToList());
            var categoriesDictionary =
                dictionariesAndCategories.Categories.GroupBy(x => x.ParentCategoryId)
                    .ToDictionary(x => x.Key == null ? "" : x.Key.ToString(), x => x.ToList());
            return
                Json(
                    new
                    {
                        type = BookTypeEnumContract.Edition,
                        //books = booksDictionary,
                        categories = categoriesDictionary
                    }, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public ActionResult SearchCriteriaText(string text, int start, int count)
        {
            var headwordContract = new WordListCriteriaContract
            {
                Key = CriteriaKey.Headword,
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract {Contains = new List<string> {text}}
                }
            };
            var headwordDescriptionContract = new WordListCriteriaContract
            {
                Key = CriteriaKey.HeadwordDescription,
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract {Contains = new List<string> {text}}
                }
            };
            var resultCriteria = new ResultCriteriaContract
            {
                Start = start,
                Count = count
            };

            var searchContract = new List<SearchCriteriaContract>
            {
                headwordContract,
                headwordDescriptionContract,
                resultCriteria
            };

            // TODO add search
            return Json(new {a = "aasdfd"});
        }

        [HttpPost]
        public ActionResult SearchCriteriaResultsCount(string json)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);

            var resultCount = m_mainServiceClient.SearchHeadwordByCriteria(listSearchCriteriaContracts);
            return Json(resultCount);
        }

        [HttpPost]
        public ActionResult SearchCriteria(string json, int start, int count)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);
            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = start,
                Count = count
            });
            
            var result = m_mainServiceClient.SearchHeadwordByCriteria(listSearchCriteriaContracts); // TODO criteria filter
            return Json(result);
        }

        [HttpPost]
        public ActionResult SearchCriteriaFulltext(string json, int start, int count)
        {
            var listSearchCriteriaContracts = DeserializeJsonSearchCriteria(json);
            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = start,
                Count = count
            });

            var result = m_mainServiceClient.SearchHeadwordByCriteria(listSearchCriteriaContracts); //TODO fulltext parameter
            return Json(result);
        }

        public ActionResult GetHeadwordDescription(string bookGuid, string xmlEntryId)
        {
            var result = m_mainServiceClient.GetDictionaryEntryByXmlId(bookGuid, xmlEntryId, OutputFormatEnumContract.Html);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadwordDescriptionFromSearch()
        {
            //TODO only mocked now
            var searchCriteriaContracts = GetMockedCriteria();

            var result = m_mainServiceClient.GetDictionaryEntryFromSearch(searchCriteriaContracts, "{08BE3E56-77D0-46C1-80BB-C1346B757BE5}", "en000001", OutputFormatEnumContract.Html);
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

        public ActionResult GetTypeaheadDictionaryHeadword(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var result = m_mainServiceClient.GetTypeaheadDictionaryHeadwords(selectedCategoryIds, selectedBookIds, query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<SearchCriteriaContract> GetMockedCriteria()
        {
            var title1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Title,
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        EndsWith = "n_k",
                        StartsWith = "Star"
                    },
                    new WordCriteriaContract
                    {
                        StartsWith = "Ele",
                        Contains = new List<string> {"slov", "st"},
                        EndsWith = "iny"
                    }
                }
            };

            var fulltext1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Fulltext,
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"alab"}
                    }
                }
            };

            return new List<SearchCriteriaContract>
            {
                title1,
                fulltext1
            };
        }
    }
}