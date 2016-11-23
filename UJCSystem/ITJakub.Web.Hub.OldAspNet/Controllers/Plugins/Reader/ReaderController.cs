﻿using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using ITJakub.Web.Hub.Models.Requests.Reader;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Controllers.Plugins.Reader
{
    public class ReaderController : BaseController
    {

        public ActionResult HasBookPageByXmlId(string bookId, string versionId)
        {
            using (var client = GetMainServiceClient())
            {
                return Json(new {HasBookPage = client.HasBookPageByXmlId(bookId, versionId)},
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBookPageByXmlId(string bookId, string pageXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                var text = client.GetBookPageByXmlId(bookId, pageXmlId, OutputFormatEnumContract.Html,
                    BookTypeEnumContract.Edition);
                return Json(new {pageText = text}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTermsOnPage(string bookId, string pageXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                var terms = client.GetTermsOnPage(bookId, pageXmlId);
                return Json(new {terms}, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetBookSearchPageByXmlId(string query, bool isQueryJson, string bookId, string pageXmlId)
        {
            IList<SearchCriteriaContract> listSearchCriteriaContracts;
            if (isQueryJson)
            {
                var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(query,
                    new ConditionCriteriaDescriptionConverter());
                listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);
            }
            else
            {
                listSearchCriteriaContracts = new List<SearchCriteriaContract>
                {
                    new WordListCriteriaContract
                    {
                        Key = CriteriaKey.Fulltext,
                        Disjunctions = new List<WordCriteriaContract>
                        {
                            new WordCriteriaContract
                            {
                                Contains = new List<string> {query}
                            }
                        }
                    }
                };
            }
            using (var client = GetMainServiceClient())
            {
                var text = client.GetEditionPageFromSearch(listSearchCriteriaContracts, bookId, pageXmlId,
                    OutputFormatEnumContract.Html);
                return Json(new {pageText = text}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBookPageList(string bookId)
        {
            using (var client = GetMainServiceClient())
            {
                var pages = client.GetBookPageList(bookId);
                return Json(new {pageList = pages}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBookContent(string bookId)
        {
            using (var client = GetMainServiceClient())
            {
                var contentItems = client.GetBookContent(bookId);
                return Json(new {content = contentItems}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddBookmark(AddBookmarkRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.AddPageBookmark(request.BookId, request.PageXmlId, HttpContext.User.Identity.Name);
            }
            return Json(new {});
        }

        public ActionResult SetBookmakTitle(SetBookmakTitleRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var successSave=client.SetPageBookmarkTitle(request.BookId, request.PageXmlId, request.Title, HttpContext.User.Identity.Name);

                if (!successSave)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                return Json(new {});
            }
        }


        public ActionResult RemoveBookmark(RemoveBookmarkRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.RemovePageBookmark(request.BookId, request.PageXmlId, HttpContext.User.Identity.Name);
            }
            return Json(new {});
        }

        public ActionResult GetAllBookmarks(string bookId)
        {
            using (var client = GetMainServiceClient())
            {
                var bookmarsList = client.GetPageBookmarks(bookId, HttpContext.User.Identity.Name);
                return Json(new {bookmarks = bookmarsList}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}