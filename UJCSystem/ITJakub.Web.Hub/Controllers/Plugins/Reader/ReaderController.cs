using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Controllers.Plugins.Reader
{
    public class ReaderController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient;
        private readonly ItJakubServiceEncryptedClient m_mainServiceEncryptedClient;

        public ReaderController()
        {
            m_mainServiceClient = new ItJakubServiceClient();
            m_mainServiceEncryptedClient = new ItJakubServiceEncryptedClient();
        }

        public ActionResult GetBookPageByXmlId(string bookId, string pageXmlId)
        {
            var text = m_mainServiceClient.GetBookPageByXmlId(bookId, pageXmlId, OutputFormatEnumContract.Html, BookTypeEnumContract.Edition);
            return Json(new { pageText = text }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTermsOnPage(string bookId, string pageXmlId)
        {
            var terms = m_mainServiceClient.GetTermsOnPage(bookId, pageXmlId);
            return Json(new { terms }, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult GetBookSearchPageByXmlId(string query,bool isQueryJson, string bookId, string pageXmlId)
        {

            IList<SearchCriteriaContract> listSearchCriteriaContracts;
            if (isQueryJson)
            {
                var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(query, new ConditionCriteriaDescriptionConverter());
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
                                Contains = new List<string>{query}
                            }
                        }
                    }
                };
            }

            var text = m_mainServiceClient.GetEditionPageFromSearch(listSearchCriteriaContracts, bookId, pageXmlId, OutputFormatEnumContract.Html);
            return Json(new { pageText = text }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBookPageList(string bookId)
        {
            var pages = m_mainServiceClient.GetBookPageList(bookId);
            return Json(new { pageList =  pages}, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetBookContent(string bookId)
        {
            var contentItems = m_mainServiceClient.GetBookContent(bookId);
            return Json(new { content = contentItems }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddBookmark(string bookId, string pageXmlId)
        {
            m_mainServiceEncryptedClient.AddPageBookmark(bookId, pageXmlId, HttpContext.User.Identity.Name);
            return Json(new {});
        }

        public ActionResult RemoveBookmark(string bookId, string pageXmlId)
        {
            m_mainServiceEncryptedClient.RemovePageBookmark(bookId, pageXmlId, HttpContext.User.Identity.Name);
            return Json(new { });
        }

        public ActionResult GetAllBookmarks(string bookId)
        {
            var bookmarsList = m_mainServiceEncryptedClient.GetPageBookmarks(bookId, HttpContext.User.Identity.Name);
            return Json(new { bookmarks = bookmarsList }, JsonRequestBehavior.AllowGet);
        }
    }
}