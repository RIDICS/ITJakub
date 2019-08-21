using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Ridics.Authentication.HttpClient.Exceptions;
using Vokabular.MainService.DataContracts;
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
            catch (MainServiceException exception)
            {
                await FillResponse(context, (int)exception.StatusCode, JsonConvert.SerializeObject(new ErrorContract { Code = exception.Code, Description = exception.Message, DescriptionParams = exception.DescriptionParams}));
            }
        }

        private async Task FillResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(message);
        }
    }
}
