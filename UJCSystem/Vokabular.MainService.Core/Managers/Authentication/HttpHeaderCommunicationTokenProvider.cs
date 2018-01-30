using System.Linq;
using Microsoft.AspNetCore.Http;
using Vokabular.RestClient.Headers;

namespace Vokabular.MainService.Core.Managers.Authentication
{
    public class HttpHeaderCommunicationTokenProvider : ICommunicationTokenProvider
    {
        private readonly IHttpContextAccessor m_httpContextAccessor;

        public HttpHeaderCommunicationTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }

        public string GetCommunicationToken()
        {
            if (m_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(CustomHttpHeaders.Authorization, out var communicationTokens))
            {
                return communicationTokens.First();
            }
            return null;
        }
    }
}