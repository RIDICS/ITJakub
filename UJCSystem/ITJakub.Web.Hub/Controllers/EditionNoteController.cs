using System.Net;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNetCore.Mvc;
using Vokabular.RestClient.Errors;
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
            var client = GetBookClient();

            try
            {
                var text = client.GetEditionNoteText(bookId, TextFormatEnumContract.Html);

                return View(new EditionNoteViewModel
                {
                    NoteText = text,
                });
            }
            catch(HttpErrorCodeException exception)
            {
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    return View(new EditionNoteViewModel());
                }

                throw;
            }
        }
    }
}