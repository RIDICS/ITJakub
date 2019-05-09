using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers
{
    public class EditionNoteController : BaseController
    {
        public EditionNoteController(CommunicationProvider communicationProvider) : base(communicationProvider)
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