using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
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
        }

        private async Task ReExecute(HttpContext context, int statusCode)
        {
            var pathString = new PathString($"/Error/{statusCode}");
            var originalPath = context.Request.Path;
            var originalQueryString = context.Request.QueryString;

            context.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature
            {
                OriginalPathBase = context.Request.PathBase.Value,
                OriginalPath = originalPath.Value,
                OriginalQueryString = originalQueryString.HasValue ? originalQueryString.Value : null
            });
            context.Request.Path = pathString;
            context.Request.QueryString = QueryString.Empty;


            try
            {
                await m_next(context);
            }
            finally
            {
                context.Request.QueryString = originalQueryString;
                context.Request.Path = originalPath;
                context.Response.StatusCode = statusCode;
                context.Features.Set<IStatusCodeReExecuteFeature>(null);
            }
        }
    }
}
