using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Helpers;
using Microsoft.AspNetCore.Mvc;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers
{
    public class EditionNoteController : BaseController
    {
        public EditionNoteController(CommunicationProvider communicationProvider, HttpErrorCodeTranslator httpErrorCodeTranslator) : base(
            communicationProvider, httpErrorCodeTranslator)
        {
        }

        public ActionResult EditionNote(long bookId)
        {
            using (var client = GetRestClient())
            {
                var text = client.GetEditionNote(bookId, TextFormatEnumContract.Html);
                ViewData["noteText"] = text;
                return View();
            }
        }
    }
}