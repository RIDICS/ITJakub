using System;
using System.Collections.Generic;
using System.Net;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Results;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.CardFiles.Controllers
{
    [Area("CardFiles")]
    public class CardFilesController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public CardFilesController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager, CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.CardFile;

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification { Area = "CardFiles", Controller = "CardFiles" };
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
            term = term == null ? string.Empty : term.ToLower();

            using (var client = GetMainServiceClient())
            {
                var cardFiles = client.GetCardFiles();
                var result = new List<SearchResultContract>();
                foreach (var cardFile in cardFiles)
                {
                    if (cardFile.Name.ToLower().Contains(term) || cardFile.Description.ToLower().Contains(term) || string.IsNullOrWhiteSpace(term))
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
                return Json(new {books = result}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextCardFilesInfo);
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback, GetEncryptedClient(), GetUserName());
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                m_feedbacksManager.FillViewModel(model, StaticTexts.TextHomeFeedback, GetFeedbackFormIdentification());
                return View(model);
            }

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.CardFiles, GetMainServiceClient(), IsUserLoggedIn(), GetUserName());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult CardFiles()
        {
            using (var client = GetMainServiceClient())
            {
                var cardFiles = client.GetCardFiles();
                return Json(new {cardFiles}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult Buckets(string cardFileId, string headword)
        {
            using (var client = GetMainServiceClient())
            {
                var buckets = string.IsNullOrEmpty(headword)
                    ? client.GetBuckets(cardFileId)
                    : client.GetBucketsWithHeadword(cardFileId, headword);
                return Json(new {buckets}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult Cards(string cardFileId, string bucketId)
        {
            using (var client = GetMainServiceClient())
            {
                var cards = client.GetCards(cardFileId, bucketId);
                return Json(new {cards}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult CardsShort(string cardFileId, string bucketId)
        {
            using (var client = GetMainServiceClient())
            {
                var cards = client.GetCardsShort(cardFileId, bucketId);
                return Json(new {cards}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult Card(string cardFileId, string bucketId, string cardId)
        {
            using (var client = GetMainServiceClient())
            {
                var card = client.GetCard(cardFileId, bucketId, cardId);
                return Json(new {card}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult Image(string cardFileId, string bucketId, string cardId, string imageId, string imageSize)
        {
            ImageSizeEnum imageSizeEnum;
            var parsingSucceeded = Enum.TryParse(imageSize, true, out imageSizeEnum);

            if (!parsingSucceeded)
            {
                return StatusCode((int) HttpStatusCode.BadRequest); // invalid image size
            }
            using (var client = GetMainServiceClient())
            {
                var imageDataStream = client.GetImage(cardFileId, bucketId, cardId, imageId, imageSizeEnum);
                return new FileStreamResult(imageDataStream, "image/jpeg");
            }
        }
    }
}