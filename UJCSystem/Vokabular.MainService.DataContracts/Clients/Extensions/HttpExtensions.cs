using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace Vokabular.MainService.DataContracts.Clients.Extensions
{
    internal static class HttpExtensions
    {
        public static string AddQueryString(this string url, string name, string value)
        {
            return QueryHelpers.AddQueryString(url, name, value);
        }

        public static string EncodeQueryString(this string query)
        {
            return UrlEncoder.Default.Encode(query);
        }
    }
}
