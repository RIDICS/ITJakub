using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Middleware
{
    public class PortalTypeMiddleware
    {
        private readonly RequestDelegate m_next;

        public PortalTypeMiddleware(RequestDelegate next)
        {
            m_next = next;
        }

        public async Task Invoke(HttpContext context, PortalTypeProvider portalTypeProvider)
        {
            var portalType = PortalTypeContract.Research; // Default value
            if (context.Request.Headers.TryGetValue(MainServiceHeaders.PortalTypeHeader, out var portalTypeValue))
            {
                portalType = Enum.Parse<PortalTypeContract>(portalTypeValue.ToString(), true);
            }

            portalTypeProvider.PortalType = portalType;
            
            await m_next(context);
        }
    }
}