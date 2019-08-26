using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;

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
            
            return RedirectToAction("Listing", "Editions",new
            {
                Area = "Editions",
                bookId,
                searchText,
                page = pageId,
            });
        }
    }
}
