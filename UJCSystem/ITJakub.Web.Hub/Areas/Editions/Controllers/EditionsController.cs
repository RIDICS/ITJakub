using System.IO;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Web.Hub.Areas.Editions.Models;

namespace ITJakub.Web.Hub.Areas.Editions.Controllers
{
    [RouteArea("Editions")]
    public class EditionsController : Controller
    {
        private readonly ItJakubServiceClient m_serviceClient;

        public EditionsController()
        {
            m_serviceClient = new ItJakubServiceClient();
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
            var book = m_serviceClient.GetBookInfo(bookId);
            return View(new BookListingModel { BookId = book.Guid, BookTitle = book.Title, BookPages = book.BookPages});
        }


        public FileResult GetBookImage(string bookId, int position)
        {
            var imageDataStream = m_serviceClient.GetBookPageImage(new BookPageImageContract
            {
                BookGuid = bookId,
                Position = position
            });
            return new FileStreamResult(imageDataStream, "image/jpeg"); //TODO resolve content type properly
        }

        public ActionResult List()
        {
            return View();
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
    }
}