using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Scalesoft.Localization.AspNetCore;
using Vokabular.RestClient.Errors;

namespace ITJakub.Web.Hub.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate m_next;
        private readonly ILocalizationService m_localization;

        public ErrorHandlingMiddleware(RequestDelegate next, ILocalizationService localization)
        {
            m_next = next;
            m_localization = localization;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await m_next(context);
            }
            catch (HttpErrorCodeException exception)
            {
                switch (exception.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        await FillResponse(context, StatusCodes.Status401Unauthorized, m_localization.Translate("unauthorized-detail", "Error"));
                        break;
                    case HttpStatusCode.Forbidden:
                        await FillResponse(context, StatusCodes.Status403Forbidden, m_localization.Translate("forbidden-detail", "Error"));
                        break;
                    default:
                        await FillResponse(context, StatusCodes.Status502BadGateway, m_localization.Translate("bad-gateway-detail", "Error"));
                        break;
                }
            }
        }

        private async Task FillResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(message);
        }
    }
}
