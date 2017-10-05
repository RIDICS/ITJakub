using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vokabular.RestClient.Errors;

namespace ITJakub.Web.Hub.AppStart.Middleware
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
            catch (HttpErrorCodeException ex)
            {
                var statusCode = (int) ex.StatusCode;
                if (statusCode > 0)
                    context.Response.StatusCode = statusCode;

                //throw; // Otherwise other middleware can change result status code
            }
            catch (CommunicationException)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}
