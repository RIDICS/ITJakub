using System.Web.Mvc;
using ITJakub.Shared.Contracts;

namespace ITJakub.Web.Hub.Controllers
{
    public class EditionNoteController : BaseController
    {
        public ActionResult EditionNote(long bookId)
        {
            using (var client = GetMainServiceClient())
            {
                var text = client.GetBookEditionNote(bookId, OutputFormatEnumContract.Html);
                ViewData["noteText"] = text;
                return View();
            }
        }
    }
}