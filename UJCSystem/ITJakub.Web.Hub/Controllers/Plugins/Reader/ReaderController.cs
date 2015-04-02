using System.Threading.Tasks;
using System.Web.Mvc;
using ITJakub.Shared.Contracts;

namespace ITJakub.Web.Hub.Controllers.Plugins.Reader
{
    public class ReaderController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient;

        public ReaderController()
        {
            m_mainServiceClient = new ItJakubServiceClient();
        }

        public async Task<ActionResult> GetBookPageByName(string bookId, string pageName)
        {
            return Json(new { pageText = await m_mainServiceClient.GetBookPageByNameAsync(bookId, pageName, OutputFormatEnumContract.Html) }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetBookPageList(string bookId)
        {
            var pages = await m_mainServiceClient.GetBookPageListAsync(bookId).ConfigureAwait(false);
            return Json(new { pageList =  pages}, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetBookPageByPosition(string bookId, int pagePosition)
        {
            return Json(new { pageText = await m_mainServiceClient.GetBookPageByPositionAsync(bookId, pagePosition, OutputFormatEnumContract.Html) }, JsonRequestBehavior.AllowGet);
        }
    }
}