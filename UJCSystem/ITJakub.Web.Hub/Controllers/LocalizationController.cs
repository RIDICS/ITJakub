using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Helpers;
using Microsoft.AspNetCore.Mvc;
using Scalesoft.Localization.AspNetCore;

namespace ITJakub.Web.Hub.Controllers
{
    public class LocalizationController : BaseController
    {
        private readonly ILocalizationService m_localizationService;
        private readonly IDictionaryService m_dictionaryService;


        public LocalizationController(CommunicationProvider communicationProvider, ILocalizationService localizationService, IDictionaryService dictionaryService, HttpErrorCodeTranslator httpErrorCodeTranslator) : base(
            communicationProvider, httpErrorCodeTranslator)
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