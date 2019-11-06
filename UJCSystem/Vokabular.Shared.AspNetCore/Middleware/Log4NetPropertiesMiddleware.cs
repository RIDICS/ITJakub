using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Vokabular.Shared.AspNetCore.Middleware
{
    public class Log4NetPropertiesMiddleware
    {
        private readonly RequestDelegate m_next;

        public Log4NetPropertiesMiddleware(RequestDelegate next)
        {
            m_next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            log4net.LogicalThreadContext.Properties["TraceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

            await m_next(context);
        }
    }
}