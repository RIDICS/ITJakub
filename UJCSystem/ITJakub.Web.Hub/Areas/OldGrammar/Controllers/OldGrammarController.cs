﻿using System.Web.Mvc;
using ITJakub.Shared.Contracts;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Areas.OldGrammar.Controllers
{
    [RouteArea("OldGrammar")]
    public class OldGrammarController : Controller
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

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            var client = new ItJakubServiceClient();
            client.CreateAnonymousFeedback(model.Text, model.Name, model.Email);
            return View("Information");
        }

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult GetTypeaheadAuthor(string query)
        {
            var result = m_serviceClient.GetTypeaheadAuthorsByBookType(query, BookTypeEnumContract.Grammar);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            var result = m_serviceClient.GetTypeaheadTitlesByBookType(query, BookTypeEnumContract.Grammar, null, null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}