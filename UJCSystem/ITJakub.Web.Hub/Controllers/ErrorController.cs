using System.Diagnostics;
using Microsoft.AspNetCore.Http;
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

        public ErrorController(ILocalizationService localization)
        {
            m_localization = localization;
        }

        [Route("Error")]
        [Route("Error/{errorCode}")]
        public IActionResult Index(string errorCode, [FromQuery] string message = null)
        {
            var errorMessage = message;
            var errorMessageDetail = string.Empty;
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = m_localization.Translate("unknown-error-msg", "Error").Value;
                if (int.TryParse(errorCode, out var errorCodeNumber))
                {
                    Response.StatusCode = errorCodeNumber;
                    switch (errorCodeNumber)
                    {
                        case StatusCodes.Status400BadRequest:
                            errorMessage = m_localization.Translate("bad-request-msg", "Error");
                            errorMessageDetail = m_localization.Translate("bad-request-detail", "Error");
                            break;
                        case StatusCodes.Status401Unauthorized:
                            errorMessage = m_localization.Translate("unauthorized-msg", "Error");
                            errorMessageDetail = m_localization.Translate("unauthorized-detail", "Error");
                            break;
                        case StatusCodes.Status403Forbidden:
                            errorMessage = m_localization.Translate("forbidden-msg", "Error");
                            errorMessageDetail = m_localization.Translate("forbidden-detail", "Error");
                            break;
                        case StatusCodes.Status404NotFound:
                            errorMessage = m_localization.Translate("not-found-msg", "Error");
                            errorMessageDetail = m_localization.Translate("not-found-detail", "Error");
                            break;
                        case StatusCodes.Status500InternalServerError:
                            errorMessage = m_localization.Translate("internal-server-error-msg", "Error");
                            errorMessageDetail = m_localization.Translate("internal-server-error-detail", "Error");
                            break;
                        case StatusCodes.Status502BadGateway:
                            errorMessage = m_localization.Translate("bad-gateway-msg", "Error");
                            errorMessageDetail = m_localization.Translate("bad-gateway-detail", "Error");
                            break;
                        case StatusCodes.Status504GatewayTimeout:
                            errorMessage = m_localization.Translate("gateway-timeout-msg", "Error");
                            errorMessageDetail = m_localization.Translate("gateway-timeout-detail", "Error");
                            break;
                    }
                }
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