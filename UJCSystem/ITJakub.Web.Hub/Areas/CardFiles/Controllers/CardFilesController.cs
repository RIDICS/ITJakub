using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System.Web.WebPages;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Results;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models;
using Microsoft.Ajax.Utilities;

namespace ITJakub.Web.Hub.Areas.CardFiles.Controllers
{
    [RouteArea("CardFiles")]
    public class CardFilesController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;

        public CardFilesController(StaticTextManager staticTextManager)
        {
            m_staticTextManager = staticTextManager;
        }

        public override BookTypeEnumContract AreaBookType
        {
            get { return BookTypeEnumContract.CardFile; }
        }

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

        public ActionResult List(string term)
        {
            return View();
        }

        public ActionResult SearchList(string term)
        {
            term = term.ToLower();

            using (var client = GetMainServiceClient())
            {
                var cardFiles = client.GetCardFiles();
                var result = new List<SearchResultContract>();
                foreach (var cardFile in cardFiles)
                {
                    if (cardFile.Name.ToLower().Contains(term) || cardFile.Description.ToLower().Contains(term) || term.IsNullOrWhiteSpace())
                    {
                        result.Add(new SearchResultContract
                        {
                            Title = cardFile.Name,
                            BookXmlId = cardFile.Id,
                            SubTitle = cardFile.Description,
                            BookType = AreaBookType
                        });
                    }
                }
                return Json(new {books = result}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextCardFilesInfo);
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeFeedback);

            var username = HttpContext.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return View(new FeedbackViewModel
                {
                    PageStaticText = pageStaticText
                });
            }
            using (var client = GetEncryptedClient())
            {
                var user = client.FindUserByUserName(username);
                var viewModel = new FeedbackViewModel
                {
                    Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                    Email = user.Email,
                    PageStaticText = pageStaticText
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            var username = HttpContext.User.Identity.Name;

            if (string.IsNullOrWhiteSpace(username))
            {
                using (var client = GetMainServiceClient())
                {
                    client.CreateAnonymousFeedback(model.Text, model.Name, model.Email, FeedbackCategoryEnumContract.CardFiles);
                }
            }
            else
            {
                using (var client = GetMainServiceClient())
                {
                    client.CreateFeedback(model.Text, username, FeedbackCategoryEnumContract.CardFiles);
                }
            }

            return View("Information");
        }

        public ActionResult CardFiles()
        {
            using (var client = GetMainServiceClient())
            {
                var cardFiles = client.GetCardFiles();
                return Json(new {cardFiles}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Buckets(string cardFileId, string headword)
        {
            using (var client = GetMainServiceClient())
            {
                var buckets = headword.IsEmpty()
                    ? client.GetBuckets(cardFileId)
                    : client.GetBucketsWithHeadword(cardFileId, headword);
                return Json(new {buckets}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Cards(string cardFileId, string bucketId)
        {
            using (var client = GetMainServiceClient())
            {
                var cards = client.GetCards(cardFileId, bucketId);
                return Json(new {cards}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CardsShort(string cardFileId, string bucketId)
        {
            using (var client = GetMainServiceClient())
            {
                var cards = client.GetCardsShort(cardFileId, bucketId);
                return Json(new {cards}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Card(string cardFileId, string bucketId, string cardId)
        {
            using (var client = GetMainServiceClient())
            {
                var card = client.GetCard(cardFileId, bucketId, cardId);
                return Json(new {card}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Image(string cardFileId, string bucketId, string cardId, string imageId, string imageSize)
        {
            ImageSizeEnum imageSizeEnum;
            var parsingSucceeded = Enum.TryParse(imageSize, true, out imageSizeEnum);

            if (!parsingSucceeded)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "invalid image size");
            }
            using (var client = GetMainServiceClient())
            {
                var imageDataStream = client.GetImage(cardFileId, bucketId, cardId, imageId, imageSizeEnum);
                return new FileStreamResult(imageDataStream, "image/jpeg");
            }
        }
    }
}