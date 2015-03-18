using System.Web.Mvc;
using ITJakub.Web.Hub.Areas.Editions.Models;

namespace ITJakub.Web.Hub.Areas.Editions.Controllers
{
    [RouteArea("Editions")]
    public class EditionsController : Controller
    {
        // GET: Editions/Editions
        public ActionResult Index()
        {
            return View("Information");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Listing(string bookId, string title)
        {
            return View(new BookListingModel { BookId = bookId, BookTitle = title });
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult TermsOfUse()
        {
            return View();
        }
    }
}