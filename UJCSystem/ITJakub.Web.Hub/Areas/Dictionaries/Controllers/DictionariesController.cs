using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
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

        [HttpPost]
        public ActionResult SearchCriteria(string json)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescription>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);
            m_mainServiceClient.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new { });

        }

        public ActionResult GetHeadwordDescription(string bookGuid, string xmlEntryId)
        {
            var result = m_mainServiceClient.GetDictionaryEntryByXmlId(bookGuid, xmlEntryId, OutputFormatEnumContract.Html);
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

        public ActionResult GetSearchResultCount(string query)
        {
            //TODO search with category filter
            var result = m_mainServiceClient.GetHeadwordSearchResultCount(query);
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchHeadword(string query, int page, int pageSize)
        {
            //TODO
            var result = m_mainServiceClient.SearchHeadword(query, new List<string> { "{08BE3E56-77D0-46C1-80BB-C1346B757BE5}" }, page, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadDictionaryHeadword(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var result = m_mainServiceClient.GetTypeaheadDictionaryHeadwords(selectedCategoryIds, selectedBookIds, query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}