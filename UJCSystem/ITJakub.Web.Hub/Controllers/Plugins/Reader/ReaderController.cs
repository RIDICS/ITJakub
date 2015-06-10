using System.Web.Mvc;
using ITJakub.Shared.Contracts;

namespace ITJakub.Web.Hub.Controllers.Plugins.Reader
{
    public class ReaderController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient;
        public ReaderController()
        {
            m_mainServiceClient = new ItJakubServiceClient();
        }

        public ActionResult GetBookPageByName(string bookId, string pageName)
        {
            var mainServiceClient = new ItJakubServiceClient();
            var text =  mainServiceClient.GetBookPageByNameAsync(bookId, pageName, OutputFormatEnumContract.Html);
            return Json(new { pageText = text }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBookPageList(string bookId)
        {
            var pages = m_mainServiceClient.GetBookPageList(bookId);
            return Json(new { pageList =  pages}, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetBookContent(string bookId)
        {
            var contentItems = m_mainServiceClient.GetBookContent(bookId);
            return Json(new { content = contentItems }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBookPageByPosition(string bookId, int pagePosition)
        {
            return Json(new { pageText = m_mainServiceClient.GetBookPageByPosition(bookId, pagePosition, OutputFormatEnumContract.Html) }, JsonRequestBehavior.AllowGet);
        }

        public void AddBookmark(string bookId, string pageName)
        {
            m_mainServiceClient.AddBookmark(bookId, pageName, HttpContext.User.Identity.Name);
        }

        public void RemoveBookmark(string bookId, string pageName)
        {
            m_mainServiceClient.RemoveBookmark(bookId, pageName, HttpContext.User.Identity.Name);
        }
    }
}