using ITJakub.Web.Hub.Core;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers.Plugins.Bibliography
{
    public class BibliographyController : BaseController
    {
        public BibliographyController(ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
        }

        // GET: Bibliography
        public ActionResult GetConfiguration()
        {
            string fullPath = "~/content/BibliographyConfiguration/configuration.json";
            return File(fullPath, "application/json", fullPath);
        }

        public ActionResult GetBookDetailInfo(long bookId)
        {
            var client = GetBookClient();
            var result = client.GetBookDetail(bookId);
            return Json(result, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult GetAudioBookDetailInfo(long bookId)
        {
            var client = GetBookClient();
            var result = client.GetAudioBookDetail(bookId);
            return Json(result, GetJsonSerializerSettingsForBiblModule());
        }
    }
}