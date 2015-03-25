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

        public ActionResult GetDictionariesWithCategories()
        {
            var dictionariesAndCategories = m_mainServiceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Dictionary);
            var booksDictionary = dictionariesAndCategories.Books.GroupBy(x => x.CategoryId).ToDictionary(x => x.Key.ToString(), x => x.ToList());
            var categoriesDictionary  = dictionariesAndCategories.Categories.GroupBy(x => x.ParentCategoryId).ToDictionary(x => x.Key == null ? "" : x.Key.ToString(), x => x.ToList());
            return Json(new { books = booksDictionary, categories = categoriesDictionary }, JsonRequestBehavior.AllowGet);
        }
    }
}