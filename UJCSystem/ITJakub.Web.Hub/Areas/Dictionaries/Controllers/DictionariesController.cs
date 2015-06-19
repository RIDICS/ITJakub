using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITJakub.Shared.Contracts;

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
            var dictionariesAndCategories = m_mainServiceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Edition);
            var booksDictionary = dictionariesAndCategories.Books.GroupBy(x => x.CategoryId).ToDictionary(x => x.Key.ToString(), x => x.ToList());
            var categoriesDictionary = dictionariesAndCategories.Categories.GroupBy(x => x.ParentCategoryId).ToDictionary(x => x.Key == null ? "" : x.Key.ToString(), x => x.ToList());
            return Json(new { type = BookTypeEnumContract.Edition, books = booksDictionary, categories = categoriesDictionary }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDictionariesWithCategories()
        {
            var dictionariesAndCategories = m_mainServiceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Dictionary);
            var booksDictionary = dictionariesAndCategories.Books.GroupBy(x => x.CategoryId).ToDictionary(x => x.Key.ToString(), x => x.ToList());
            var categoriesDictionary  = dictionariesAndCategories.Categories.GroupBy(x => x.ParentCategoryId).ToDictionary(x => x.Key == null ? "" : x.Key.ToString(), x => x.ToList());
            return Json(new { type = BookTypeEnumContract.Dictionary, books = booksDictionary, categories = categoriesDictionary }, JsonRequestBehavior.AllowGet);
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
            var jsonString = searchData;
            return Json(new {});
        }
    }

    //TODO move to plugins directory
    public class WordCriteriaDescription
    {
        public string startsWith { get; set; }
        public IList<string> contains { get; set; }
        public string endsWith { get; set; }
    }

    public class ConditionCriteriaDescription
    {
        public IList<WordCriteriaDescription> wordCriteriaDescription { get; set; }
        public string searchType { get; set; }
    }
}