using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using ITJakub.Web.Hub.DataContracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vokabular.MainService.DataContracts;
using Vokabular.RestClient.Errors;

namespace ITJakub.Web.Hub.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate m_next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            m_next = next;
        }

        private const string RequestedWithHeader = "X-Requested-With";
        private const string XmlHttpRequest = "XMLHttpRequest";

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await m_next(context);
            }
            catch (HttpErrorCodeException exception)
            {
                int statusCode;
                switch (exception.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        statusCode = StatusCodes.Status401Unauthorized;
                        break;
                    case HttpStatusCode.Forbidden:
                        statusCode = StatusCodes.Status403Forbidden;
                        break;
                    default:
                        statusCode = StatusCodes.Status502BadGateway;
                        break;
                }

                await ReExecute(context, statusCode);
            }
            catch (MainServiceException exception)
            {
                if (IsAjaxRequest(context.Request))
                {
                    context.Response.StatusCode = (int) exception.StatusCode;
                    var result = new ErrorContract
                    {
                        ErrorMessage = exception.Description
                    };

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(result,
                        new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()}));
                }
                else
                {
                    await ReExecute(context, (int) exception.StatusCode, exception.Description);
                }
            }
        }

        private async Task ReExecute(HttpContext context, int statusCode, string errorMessage = null)
        {
            var pathString = new PathString($"/Error/{statusCode}");
            var queryString = errorMessage == null ? QueryString.Empty : new QueryString($"?message={HttpUtility.UrlEncode(errorMessage)}");

            var originalPath = context.Request.Path;
            var originalQueryString = context.Request.QueryString;

            context.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature
            {
                OriginalPathBase = context.Request.PathBase.Value,
                OriginalPath = originalPath.Value,
                OriginalQueryString = originalQueryString.HasValue ? originalQueryString.Value : null
            });
            context.Request.Path = pathString;
            context.Request.QueryString = queryString;


            try
            {
                await m_next(context);
            }
            finally
            {
                context.Request.QueryString = originalQueryString;
                context.Request.Path = originalPath;
                context.Features.Set<IStatusCodeReExecuteFeature>(null);
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            if (request.Headers != null)
            {
                return request.Headers[RequestedWithHeader] == XmlHttpRequest;
            }

            return false;
        }
    }
}