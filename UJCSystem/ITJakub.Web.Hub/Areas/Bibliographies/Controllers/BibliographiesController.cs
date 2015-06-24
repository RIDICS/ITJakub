﻿using System.Collections.Generic;
using System.Web.Mvc;
using ITJakub.Shared.Contracts;

namespace ITJakub.Web.Hub.Areas.Bibliographies.Controllers
{
    [RouteArea("Bibliographies")]
    public class BibliographiesController : Controller
    {

        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        public ActionResult Index()
        {
            return View("Information");
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

        public ActionResult Search(string term)
        {
            IEnumerable<SearchResultContract> listBooks = m_serviceClient.Search(term);
            foreach (var list in listBooks)
            {
                list.CreateTimeString = list.CreateTime.ToString();
            }
            return Json(new { books = listBooks }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetTypeaheadAuthor(string query)
        {
            var result = m_serviceClient.GetTypeaheadAuthors(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            var result = m_serviceClient.GetTypeaheadTitles(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}