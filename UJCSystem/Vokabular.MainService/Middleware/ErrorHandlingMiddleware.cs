using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ridics.Authentication.HttpClient.Exceptions;
using Vokabular.MainService.Core.Errors;
using Vokabular.RestClient.Contracts;

namespace Vokabular.MainService.Middleware
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
            catch (AuthServiceApiException exception)
            {
                await FillResponse(context, exception.StatusCode,JsonConvert.SerializeObject(new ErrorContract{Code = exception.Code, Description = exception.Message}));
            }
            catch (AuthServiceException exception)
            {
                await FillResponse(context, exception.StatusCode, JsonConvert.SerializeObject(new ErrorContract { Description = exception.Message }));
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
