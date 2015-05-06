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
        public ActionResult Cards(string cardFileId, string bucketId)
        {
            var cards = m_serviceClient.GetCards(cardFileId, bucketId);
            return Json(new {cards}, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Card(string cardFileId, string bucketId, string cardId)
        {
            var card = m_serviceClient.GetCard(cardFileId, bucketId, cardId);
            return Json(new {card}, JsonRequestBehavior.AllowGet);
        }    
    
        public FileResult Image(string cardFileId, string bucketId, string cardId, string imageId, string imageSize)
        {
            var imageDataStream = m_serviceClient.GetImage(cardFileId, bucketId, cardId, imageId, imageSize);
            return new FileStreamResult(imageDataStream, "image/jpeg"); //TODO resolve content type properly
        }
    }
}