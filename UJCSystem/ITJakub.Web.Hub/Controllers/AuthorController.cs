using System.Collections.Generic;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.Web.Hub.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        public ActionResult GetAllAuthors()
        {
            IEnumerable<AuthorDetailContract> authors = m_serviceClient.GetAllAuthors();
            return Json(new { Authors = authors }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateAuthor(IEnumerable<AuthorInfoContract> authorInfos)
        {
            int authorId = m_serviceClient.CreateAuthor(authorInfos);
            return Json(new {AuthorId = authorId});
        }

        public ActionResult AssignAuthorsToBook(string bookGuid, string bookVersionGuid, int[] authorIds)
        {
            if (bookGuid == null || bookVersionGuid == null)
            {
                return Json(new {Error = "Cannot assigns author(s) to book. BookGuid and BookversionId could not be empty"});
            }
            m_serviceClient.AssignAuthorsToBook(bookGuid, bookVersionGuid, authorIds);
            return Json(new { });
        }
    }
}