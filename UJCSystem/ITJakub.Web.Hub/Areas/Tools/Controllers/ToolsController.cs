﻿using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts.Type;
using ITJakub.Web.Hub.Options;

namespace ITJakub.Web.Hub.Areas.Tools.Controllers
{
    [LimitedAccess(PortalType.ResearchPortal)]
    [Area("Tools")]
    public class ToolsController : BaseController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public ToolsController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager, ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification {Area = "Tools", Controller = "Tools"};
        }

        public ActionResult Index()
        {
            return List();
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextToolsInfo, "tools");
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

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.Tools, PortalTypeValue, IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult List()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextToolsList, "tools");
            return View("List", pageStaticText);
        }

    }
}