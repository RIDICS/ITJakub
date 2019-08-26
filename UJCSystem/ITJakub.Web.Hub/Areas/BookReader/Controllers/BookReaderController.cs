using System;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.BookReader.Controllers
{
    [Area("BookReader")]
    public class BookReaderController : BaseController
    {
        public BookReaderController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public ActionResult Listing(long? bookId, string searchText, string pageId)
        {
            if (bookId == null)
            {
                return BadRequest();
            }

            var client = GetProjectClient();
            var snapshotInfo = client.GetLatestPublishedSnapshot(bookId.Value);

            if (snapshotInfo == null)
            {
                return NotFound();
            }

            switch (snapshotInfo.DefaultBookType)
            {
                case BookTypeEnumContract.Edition:
                case BookTypeEnumContract.ProfessionalLiterature:
                case BookTypeEnumContract.TextBank:
                    return RedirectToAction("Listing", "Editions", new
                    {
                        Area = "Editions",
                        bookId,
                        searchText,
                        page = pageId,
                    });
                case BookTypeEnumContract.Dictionary:
                    return RedirectToAction("Listing", "Dictionaries", new
                    {
                        Area = "Dictionaries",
                        books = $"[{bookId}]"
                    });
                case BookTypeEnumContract.Grammar:
                    return RedirectToAction("Listing", "OldGrammar", new
                    {
                        Area = "OldGrammar",
                        bookId,
                        searchText,
                        page = pageId,
                    });
                case BookTypeEnumContract.BibliographicalItem:
                    // No direct access to book (content doesn't exist), but at least display the bibliography list
                    return RedirectToAction("Search", "Bibliographies", new
                    {
                        Area = "Bibliographies",
                    });
                case BookTypeEnumContract.AudioBook:
                    // No direct access to book, but at least display the audio books list
                    return RedirectToAction("List", "AudioBooks", new
                    {
                        Area = "AudioBooks",
                    });
                case BookTypeEnumContract.CardFile:
                    return BadRequest();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
