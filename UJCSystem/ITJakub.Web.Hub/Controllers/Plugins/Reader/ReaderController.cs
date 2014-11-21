using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers.Plugins.Reader
{
    public class ReaderController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient;

        public ReaderController(ItJakubServiceClient mainServiceClient)
        {
            m_mainServiceClient = mainServiceClient;
        }

        public ActionResult GetBookPageByName(string bookId, string pageName)
        {
            return Json(new { pageText = m_mainServiceClient.GetBookPageByName(bookId, pageName, "html") }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBookPageList(string bookId)
        {
            return Json(new { pageText = m_mainServiceClient.GetBookPageList(bookId) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBookPageByPosition(string bookId, int pagePosition)
        {
            return Json(new { pageText = m_mainServiceClient.GetBookPageByPosition(bookId, pagePosition, "html") }, JsonRequestBehavior.AllowGet);
        }
    }
}