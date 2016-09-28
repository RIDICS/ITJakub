using System.Collections.Generic;
using AutoMapper;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using ITJakub.Web.Hub.Models.Requests.Reader;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Controllers.Plugins.Reader
{
    public class ReaderController : BaseController
    {
        public ReaderController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public ActionResult HasBookPageByXmlId(string bookId, string versionId)
        {
            using (var client = GetMainServiceClient())
            {
                return Json(new {HasBookPage = client.HasBookPageByXmlId(bookId, versionId)});
            }
        }

        public ActionResult GetBookPageByXmlId(string bookId, string pageXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                var text = client.GetBookPageByXmlId(bookId, pageXmlId, OutputFormatEnumContract.Html,
                    BookTypeEnumContract.Edition);
                return Json(new {pageText = text});
            }
        }

        public ActionResult GetTermsOnPage(string bookId, string pageXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                var terms = client.GetTermsOnPage(bookId, pageXmlId);
                return Json(new {terms});
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
                return Json(new {pageText = text});
            }
        }

        public ActionResult GetBookPageList(string bookId)
        {
            using (var client = GetMainServiceClient())
            {
                var pages = client.GetBookPageList(bookId);
                return Json(new {pageList = pages});
            }
        }

        public ActionResult GetBookContent(string bookId)
        {
            using (var client = GetMainServiceClient())
            {
                var contentItems = client.GetBookContent(bookId);
                return Json(new {content = contentItems});
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
                    return BadRequest();
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
                return Json(new {bookmarks = bookmarsList});
            }
        }
    }
}