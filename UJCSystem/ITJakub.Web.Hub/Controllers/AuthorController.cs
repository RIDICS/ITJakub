using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class AuthorController : BaseController
    {
        public ActionResult GetAllAuthors()
        {
            using (var client = GetMainServiceClient())
            {
                var authors = client.GetAllAuthors();
                return Json(new {Authors = authors}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}