using System.Web.Mvc;
using ITJakub.Web.Hub.Areas.Editions.Models;

namespace ITJakub.Web.Hub.Areas.Editions.Controllers
{
    [RouteArea("Editions")]
    public class EditionsController : Controller
    {
        private ItJakubServiceClient m_mainServiceClient;

        public EditionsController()
        {
            m_mainServiceClient = new ItJakubServiceClient();
        }

        // GET: Editions/Editions
        public ActionResult Index()
        {
            return View("Information");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Listing(string bookId)
        {
            var book = m_mainServiceClient.GetBookInfo(bookId);
            return View(new BookListingModel { BookId = book.Guid, BookTitle = book.Title, BookPages = book.BookPages});
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