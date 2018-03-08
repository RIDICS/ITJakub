using System;
using ITJakub.Web.Hub.Core.Communication;
using Localization.AspNetCore.Service;
using Localization.CoreLibrary.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class LocalizationController : BaseController
    {
        private readonly IDictionary m_dictionary;


        public LocalizationController(CommunicationProvider communicationProvider, IDictionary dictionary) : base(communicationProvider)
        {
            m_dictionary = dictionary;
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            RequestCulture requestCulture = new RequestCulture(culture);
            HttpContext.Request.
                HttpContext.Response.Cookies.Append(
                    "Localization.Culture",
                    requestCulture.Culture.Name,
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1)
                    }
                );

            return LocalRedirect(returnUrl);
        }

        public ActionResult Dictionary(string scope)
        {
            return Json(m_dictionary.GetDictionary(scope, LocTranslationSource.Auto));
        }
    }
}