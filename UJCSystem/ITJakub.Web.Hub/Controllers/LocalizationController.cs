using System;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class LocalizationController : BaseController
    {
        public LocalizationController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
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
    }
}