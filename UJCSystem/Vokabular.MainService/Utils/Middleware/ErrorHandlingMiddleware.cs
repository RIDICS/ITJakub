using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vokabular.MainService.Core.Errors;

namespace Vokabular.MainService.Utils.Middleware
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
            catch (ArgumentException exception)
            {
                await FillResponse(context, StatusCodes.Status400BadRequest, exception.Message);
            }
            catch (AuthenticationException exception)
            {
                await FillResponse(context, StatusCodes.Status401Unauthorized, exception.Message);
            }
            catch (UnauthorizedException exception)
            {
                await FillResponse(context, StatusCodes.Status403Forbidden, exception.Message);
            }
        }

        private async Task FillResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(message);
        }
    }
}
