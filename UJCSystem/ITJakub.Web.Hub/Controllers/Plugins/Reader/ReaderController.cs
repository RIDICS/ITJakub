using System;
using System.Web.Helpers;
using System.Web.Mvc;
using ITJakub.Shared.Contracts;

namespace ITJakub.Web.Hub.Controllers.Plugins.Reader
{
    public class ReaderController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient;
        private readonly ItJakubServiceEncryptedClient m_mainServiceEncryptedClient;

        public ReaderController()
        {
            m_mainServiceClient = new ItJakubServiceClient();
            m_mainServiceEncryptedClient = new ItJakubServiceEncryptedClient();
        }

        public ActionResult GetBookPageByXmlId(string bookId, string pageXmlId)
        {
            var mainServiceClient = new ItJakubServiceClient();
            var text = mainServiceClient.GetBookPageByXmlId(bookId, pageXmlId, OutputFormatEnumContract.Html, BookTypeEnumContract.Edition);
            return Json(new { pageText = text }, JsonRequestBehavior.AllowGet);
        }

        //TODO add json or text parameter (2 methods) and move it to right controller by module
        public ActionResult GetBookSearchPageByXmlId(string bookId, string pageXmlId)
        {
            var mainServiceClient = new ItJakubServiceClient();
            throw new NotImplementedException();
            var text = mainServiceClient.GetBookPageByXmlId(bookId, pageXmlId, OutputFormatEnumContract.Html, BookTypeEnumContract.Edition); //TODO change on method for retrieve page with search result
            return Json(new { }, JsonRequestBehavior.AllowGet);
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

        public ActionResult AddBookmark(string bookId, string pageXmlId)
        {
            m_mainServiceEncryptedClient.AddBookmark(bookId, pageXmlId, HttpContext.User.Identity.Name);
            return Json(new {});
        }

        public ActionResult RemoveBookmark(string bookId, string pageXmlId)
        {
            m_mainServiceEncryptedClient.RemoveBookmark(bookId, pageXmlId, HttpContext.User.Identity.Name);
            return Json(new { });
        }

        public ActionResult GetAllBookmarks(string bookId)
        {
            var bookmarsList = m_mainServiceEncryptedClient.GetPageBookmarks(bookId, HttpContext.User.Identity.Name);
            return Json(new { bookmarks = bookmarsList }, JsonRequestBehavior.AllowGet);
        }
    }
}