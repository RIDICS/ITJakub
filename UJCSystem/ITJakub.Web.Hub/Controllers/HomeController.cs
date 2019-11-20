﻿using System.Linq;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers
{
    public class HomeController : BaseController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public HomeController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager,
            ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification {Area = string.Empty, Controller = "Home"};
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeAbout, "home");
            return View(staticTextViewModel);
        }

        public ActionResult Contacts()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeContacts, "home");
            return View(staticTextViewModel);
        }

        public ActionResult Copyright()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeCopyright, "home");
            return View(staticTextViewModel);
        }

        public ActionResult Feedback()
        {
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback,
                IsUserLoggedIn(), "home", User);
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

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.None, IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult HowToCite()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeHowToCite, "home");
            return View(staticTextViewModel);
        }

        public ActionResult Links()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeLinks, "home");
            return View(staticTextViewModel);
        }

        public ActionResult Support()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeSupport, "home");
            return View(staticTextViewModel);
        }

        public ActionResult GetTypeaheadAuthor(string query)
        {
            var client = GetCodeListClient();
            var result = client.GetOriginalAuthorAutocomplete(query);
            var response = result.Select(x => x.FirstName != null ? $"{x.FirstName} {x.LastName}" : x.LastName).ToList();
            return Json(response);
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            BookTypeEnumContract? bookType = null;
            if (PortalTypeValue == PortalTypeContract.Community)
            {
                bookType = BookTypeEnumContract.Edition;
            }
            
            var client = GetMetadataClient();
            var result = client.GetTitleAutocomplete(query, projectType: GetDefaultProjectType(), bookType: bookType);
            return Json(result);
        }

        public ActionResult GetTypeaheadDictionaryHeadword(string query)
        {
            var client = GetBookClient();
            var result = client.GetHeadwordAutocomplete(query, GetDefaultProjectType());
            return Json(result);
        }
    }
}