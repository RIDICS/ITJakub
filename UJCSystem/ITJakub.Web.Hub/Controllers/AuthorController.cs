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
            return Json(new {Authors = authors});
        }

        public ActionResult CreateAuthor(IEnumerable<AuthorInfoContract> authorInfos)
        {
            int authorId = m_serviceClient.CreateAuthor(authorInfos);
            return Json(new {AuthorId = authorId});
        }
    }
}