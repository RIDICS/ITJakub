using ITJakub.SearchService.DataContracts.Types;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class EditionNoteController : BaseController
    {
        public EditionNoteController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

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