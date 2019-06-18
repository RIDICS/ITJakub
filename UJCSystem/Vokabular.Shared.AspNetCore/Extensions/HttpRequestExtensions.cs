using System;
using Microsoft.AspNetCore.Http;

namespace Vokabular.Shared.AspNetCore.Extensions
{
    public static class HttpRequestExtensions
    {
        public static Uri GetAppBaseUrl(this HttpRequest request)
        {
            var urlBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.PathBase,
            };

            if (request.Host.Port.HasValue)
            {
                urlBuilder.Port = request.Host.Port.Value;
            }

            return urlBuilder.Uri;
        }
    }
}