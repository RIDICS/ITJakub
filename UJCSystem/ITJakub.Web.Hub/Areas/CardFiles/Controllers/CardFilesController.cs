using System.Web.Mvc;
using System.Web.WebPages;

namespace ITJakub.Web.Hub.Areas.CardFiles.Controllers
{
    [RouteArea("CardFiles")]
    public class CardFilesController : Controller
    {
        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Listing()
        {
            return View();
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

        public ActionResult Feedback()
        {
            return View();
        }

        public ActionResult CardFiles()
        {
            var cardFiles = m_serviceClient.GetCardFiles();
            return Json(new {cardFiles}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Buckets(string cardFileId, string headword)
        {
            var buckets = headword.IsEmpty()
                ? m_serviceClient.GetBuckets(cardFileId)
                : m_serviceClient.GetBucketsWithHeadword(cardFileId, headword);
            return Json(new {buckets}, JsonRequestBehavior.AllowGet);
        }
    }
}