using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class AuthorController : BaseController
    {
        public AuthorController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public ActionResult GetAllAuthors()
        {
            using (var client = GetMainServiceClient())
            {
                var authors = client.GetAllAuthors();
                return Json(new {Authors = authors});
            }
        }
    }
}