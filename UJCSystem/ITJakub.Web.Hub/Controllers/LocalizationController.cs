using ITJakub.Web.Hub.Core;
using Microsoft.AspNetCore.Mvc;
using Scalesoft.Localization.AspNetCore;

namespace ITJakub.Web.Hub.Controllers
{
    public class LocalizationController : BaseController
    {
        private readonly ILocalizationService m_localizationService;
        private readonly IDictionaryService m_dictionaryService;


        public LocalizationController(ControllerDataProvider controllerDataProvider, ILocalizationService localizationService, IDictionaryService dictionaryService) : base(controllerDataProvider)
        {
            m_localizationService = localizationService;
            m_dictionaryService = dictionaryService;
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            m_localizationService.SetCulture(culture);
            
            return LocalRedirect(returnUrl);
        }

        public ActionResult Dictionary(string scope)
        {
            return Json(m_dictionaryService.GetDictionary(scope));
        }
    }
}