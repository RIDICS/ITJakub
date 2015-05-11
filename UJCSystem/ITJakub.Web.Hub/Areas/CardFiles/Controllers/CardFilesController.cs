using System;
using System.Net;
using System.Web.Mvc;
using System.Web.WebPages;
using ITJakub.ITJakubService.DataContracts;

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
        public ActionResult CardsShort(string cardFileId, string bucketId)
        {
            var cards = m_serviceClient.GetCardsShort(cardFileId, bucketId);
            return Json(new {cards}, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Card(string cardFileId, string bucketId, string cardId)
        {
            var card = m_serviceClient.GetCard(cardFileId, bucketId, cardId);
            return Json(new {card}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Image(string cardFileId, string bucketId, string cardId, string imageId, string imageSize)
        {
            ImageSizeEnum imageSizeEnum;
            var parsingSucceeded = Enum.TryParse(imageSize, true, out imageSizeEnum);

            if (!parsingSucceeded)
            {
               return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "invalid image size");
            }

            var imageDataStream = m_serviceClient.GetImage(cardFileId, bucketId, cardId, imageId, imageSizeEnum);
            return new FileStreamResult(imageDataStream, "image/jpeg");
        }
    }
}