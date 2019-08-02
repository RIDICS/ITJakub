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

        public ActionResult Listing(long bookId, string searchText, string pageId)
        {
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
