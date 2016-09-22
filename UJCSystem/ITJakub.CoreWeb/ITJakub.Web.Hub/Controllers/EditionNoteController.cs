using ITJakub.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

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