using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers.Plugins.Bibliography
{
    public class BibliographyController : BaseController
    {
        // GET: Bibliography
        public ActionResult GetConfiguration()
        {
            string fullPath = Server.MapPath("~/Content/Plugins/Bibliography/configuration.json");
            return File(fullPath, "application/json", fullPath);
        }

        public ActionResult GetBookDetailInfo(long bookId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetBookDetailInfoById(bookId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
    }
}