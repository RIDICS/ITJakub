using System.Diagnostics;
using ITJakub.Web.Hub.Helpers;
using Microsoft.AspNetCore.Mvc;
using Ridics.Core.Structures.Shared;
using Scalesoft.Localization.AspNetCore;

namespace ITJakub.Web.Hub.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorController : Controller
    {
        private readonly ILocalizationService m_localization;
        private readonly HttpErrorCodeTranslator m_httpErrorCodeTranslator;

        public ErrorController(ILocalizationService localization, HttpErrorCodeTranslator httpErrorCodeTranslator)
        {
            m_localization = localization;
            m_httpErrorCodeTranslator = httpErrorCodeTranslator;
        }

        [Route("Error")]
        [Route("Error/{errorCode}")]
        public IActionResult Index(string errorCode)
        {
            var errorMessage = m_localization.Translate("unknown-error-msg", "Error").Value;
            var errorMessageDetail = string.Empty;

            if (int.TryParse(errorCode, out var errorCodeNumber))
            {
                var errorContract = m_httpErrorCodeTranslator.GetMessageFromErrorCode(errorCodeNumber);
                errorMessage = errorContract.ErrorMessage;
                errorMessageDetail = errorContract.ErrorMessageDetail;
            }

            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                ErrorMessageDetail = errorMessageDetail,
            });
        }
    }
}