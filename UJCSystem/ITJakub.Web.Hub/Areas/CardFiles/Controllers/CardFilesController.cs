using System;
using System.Collections.Generic;
using System.Net;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Helpers;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts.CardFile;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Search.Old;
using Vokabular.Shared.DataContracts.Types;
using ITJakub.Web.Hub.Options;

namespace ITJakub.Web.Hub.Areas.CardFiles.Controllers
{
    [LimitedAccess(PortalType.ResearchPortal)]
    [Area("CardFiles")]
    public class CardFilesController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public CardFilesController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager, CommunicationProvider communicationProvider, 
            HttpErrorCodeTranslator httpErrorCodeTranslator) : base(communicationProvider, httpErrorCodeTranslator)
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

            using (var client = GetRestClient())
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
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextCardFilesInfo, "cardfiles");
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback, IsUserLoggedIn(), "home", User);
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                m_feedbacksManager.FillViewModel(model, StaticTexts.TextHomeFeedback, "home", GetFeedbackFormIdentification());
                return View(model);
            }

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.CardFiles, IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult CardFiles()
        {
            using (var client = GetRestClient())
            {
                var cardFiles = client.GetCardFiles();
                return Json(new {cardFiles}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult Buckets(string cardFileId, string headword)
        {
            using (var client = GetRestClient())
            {
                var buckets = client.GetBuckets(cardFileId, headword);
                return Json(new {buckets}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult Cards(string cardFileId, string bucketId)
        {
            using (var client = GetRestClient())
            {
                var cards = client.GetCards(cardFileId, bucketId);
                return Json(new {cards}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult CardsShort(string cardFileId, string bucketId)
        {
            using (var client = GetRestClient())
            {
                var cards = client.GetCardsShort(cardFileId, bucketId);
                return Json(new {cards}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult Card(string cardFileId, string bucketId, string cardId)
        {
            using (var client = GetRestClient())
            {
                var card = client.GetCard(cardFileId, bucketId, cardId);
                return Json(new {card}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult Image(string cardFileId, string bucketId, string cardId, string imageId, string imageSize)
        {
            CardImageSizeEnumContract imageSizeEnum;
            var parsingSucceeded = Enum.TryParse(imageSize, true, out imageSizeEnum);

            if (!parsingSucceeded)
            {
                return StatusCode((int) HttpStatusCode.BadRequest); // invalid image size
            }
            using (var client = GetRestClient())
            {
                var imageData = client.GetCardImage(cardFileId, bucketId, cardId, imageId, imageSizeEnum);
                return File(imageData.Stream, imageData.MimeType, imageData.FileName, imageData.FileSize);
            }
        }
    }
}