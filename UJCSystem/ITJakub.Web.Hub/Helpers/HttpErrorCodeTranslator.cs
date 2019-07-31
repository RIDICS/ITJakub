using System.Net;
using ITJakub.Web.Hub.DataContracts;
using Microsoft.AspNetCore.Http;
using Scalesoft.Localization.AspNetCore;

namespace ITJakub.Web.Hub.Helpers
{
    public class HttpErrorCodeTranslator
    {
        private readonly ILocalizationService m_localization;

        public HttpErrorCodeTranslator(ILocalizationService localizationService)
        {
            m_localization = localizationService;
        }

        public ErrorContract GetMessageFromErrorCode(HttpStatusCode statusCode)
        {
            return GetMessageFromErrorCode((int)statusCode);
        }

        public ErrorContract GetMessageFromErrorCode(int errorCodeNumber)
        {
            string errorMessage;
            string errorMessageDetail;

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
                default:
                    errorMessage = m_localization.Translate("unknown-error-msg", "Error");
                    errorMessageDetail = string.Empty;
                    break;
            }

            return new ErrorContract
            {
                ErrorMessage = errorMessage,
                ErrorMessageDetail = errorMessageDetail
            };
        }
    }
}