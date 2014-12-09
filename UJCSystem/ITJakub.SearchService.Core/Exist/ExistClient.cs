using System;
using System.IO;
using System.Net;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistClient : IExistClient
    {
        private readonly UriCache m_uriCache;
        private readonly WebClient m_webClient;

        public ExistClient(UriCache uriCache, ExistConnectionSettingsSkeleton connectionSettings)
        {
            m_uriCache = uriCache;
            m_webClient = new WebClient
            {
                Credentials = new NetworkCredential(connectionSettings.DBUser, connectionSettings.DBPassword)
            };
        }

        public Stream GetPageList(string documentId, string xslPath)
        {
            string uriTemplate = m_uriCache.GetUriForMethod();
            if (!string.IsNullOrEmpty(xslPath))
            {
                AddXslParam(uriTemplate, xslPath);
            }
            Uri uri = SetParamsToUri(uriTemplate, documentId);
            return m_webClient.OpenRead(uri);
        }

        public Stream GetPageList(string documentId)
        {
            return GetPageList(documentId, null);
        }

        private static Uri SetParamsToUri(string uriTemplate, params object[] args)
        {
            return new Uri(string.Format(uriTemplate, args));
        }

        private static string AddXslParam(string uriTemplate, string xslPath)
        {
            return string.Format("{0}&_xsl={1}", uriTemplate, xslPath);
        }
    }
}