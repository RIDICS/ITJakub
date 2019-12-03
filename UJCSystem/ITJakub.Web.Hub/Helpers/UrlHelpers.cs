using System;
using System.Linq;
using ITJakub.Web.Hub.Options;
using Microsoft.AspNetCore.Http;

namespace ITJakub.Web.Hub.Helpers
{
    public static class UrlHelpers
    {
        public static string ConvertCurrentUrlToSecondPortal(HttpRequest httpRequest, PortalOption option)
        {
            var uriBuilder = new UriBuilder(option.GetSecondPortalUrl());
            var actionPath = httpRequest.Path;

            if (string.IsNullOrEmpty(uriBuilder.Path))
            {
                uriBuilder.Path = actionPath;
            }
            else if (uriBuilder.Path.Last() == '/')
            {
                uriBuilder.Path = $"{uriBuilder.Path.Substring(0, uriBuilder.Path.Length - 1)}{actionPath}";
            }
            else
            {
                uriBuilder.Path = $"{uriBuilder.Path}{actionPath}";
            }

            return uriBuilder.ToString();
        }
    }
}
