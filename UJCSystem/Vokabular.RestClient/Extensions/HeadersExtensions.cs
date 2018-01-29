using System;
using System.Linq;
using System.Net.Http.Headers;
using Vokabular.RestClient.Headers;

namespace Vokabular.RestClient.Extensions
{
    internal static class HeadersExtensions
    {
        public static int GetTotalCountHeader(this HttpResponseHeaders responseHeaders)
        {
            if (responseHeaders.TryGetValues(CustomHttpHeaders.TotalCount, out var values))
            {
                return Convert.ToInt32(values.First());
            }

            return -1;
        }
    }
}
