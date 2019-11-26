using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.DataContracts;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers.Plugins.Bibliography
{
    public class BibliographyController : BaseController
    {
        public BibliographyController(ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
        }
        
        public ActionResult GetBookDetailInfo(long bookId)
        {
            var client = GetBookClient();
            var result = client.GetBookDetail(bookId);
            var resultContract = Mapper.Map<SearchResultDetailExtendedContract>(result);
            return Json(resultContract, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult GetAudioBookDetailInfo(long bookId)
        {
            var client = GetBookClient();
            var result = client.GetAudioBookDetail(bookId);
            return Json(result, GetJsonSerializerSettingsForBiblModule());
        }
    }
}