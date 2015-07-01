using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;

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

        public ActionResult Passwords()
        {
            return View();
        }

        //public ActionResult SaveItem(string itemmType, string itemId, string bookType)
        //{
        //    var actionType = action == "add" ? ActionType.Save : ActionType.Delete;
        //    var itemType = action == "add" ? ItemmTypeContract.Book : ItemmTypeContract.Category;

        //    m_mainServiceClient.SaveFavorite(action, bookType);

        //}

        public ActionResult GetTextWithCategories()
        {
            var dictionariesAndCategories =
                m_mainServiceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Edition);
            var booksDictionary =
                dictionariesAndCategories.Books.GroupBy(x => x.CategoryId)
                    .ToDictionary(x => x.Key.ToString(), x => x.ToList());
            var categoriesDictionary =
                dictionariesAndCategories.Categories.GroupBy(x => x.ParentCategoryId)
                    .ToDictionary(x => x.Key == null ? "" : x.Key.ToString(), x => x.ToList());
            return
                Json(
                    new
                    {
                        type = BookTypeEnumContract.Edition,
                        books = booksDictionary,
                        categories = categoriesDictionary
                    }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDictionariesWithCategories()
        {
            var dictionariesAndCategories =
                m_mainServiceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Dictionary);
            var booksDictionary =
                dictionariesAndCategories.Books.GroupBy(x => x.CategoryId)
                    .ToDictionary(x => x.Key.ToString(), x => x.ToList());
            var categoriesDictionary =
                dictionariesAndCategories.Categories.GroupBy(x => x.ParentCategoryId)
                    .ToDictionary(x => x.Key == null ? "" : x.Key.ToString(), x => x.ToList());
            return
                Json(
                    new
                    {
                        type = BookTypeEnumContract.Dictionary,
                        books = booksDictionary,
                        categories = categoriesDictionary
                    }, JsonRequestBehavior.AllowGet);
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

        public ActionResult SearchCriteria(IList<ConditionCriteriaDescription> searchData)
        {
            var wordListCriteriaContracts = Mapper.Map<IList<WordListCriteriaContract>>(searchData);
            m_mainServiceClient.SearchByCriteria(wordListCriteriaContracts);
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSearchResultCount(string query)
        {
            //TODO return correct count
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchHeadword(string query, int page, int pageSize)
        {
            //TODO
            var result = m_mainServiceClient.SearchHeadword(query, new List<string> { "{08BE3E56-77D0-46C1-80BB-C1346B757BE5}" }, page, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadDictionaryHeadword(string query)
        {
            var result = m_mainServiceClient.GetTypeaheadDictionaryHeadwords(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}