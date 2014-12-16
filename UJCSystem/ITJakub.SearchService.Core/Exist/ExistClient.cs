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

        #region Helpers

        private static Uri SetParamsToUri(string uriTemplate, params object[] args)
        {
            return new Uri(string.Format(uriTemplate, args));
        }

        private static string AddXslParam(string uriTemplate, string xslPath)
        {
            return string.Format("{0}&_xsl={1}", uriTemplate, xslPath); //TODO add check for "?"
        }

        private static Uri GetCompleteUri(CommunicationInfo commInfo, string xslPath, params object[] args)
        {
            string uriTemplate = commInfo.UriTemplate;
            if (!string.IsNullOrEmpty(xslPath))
            {
                uriTemplate = AddXslParam(uriTemplate, xslPath);
            }
            return SetParamsToUri(uriTemplate, args);
        }

        #endregion

        public Stream GetPageList(string bookId, string versionId)
        {
            return GetPageList(bookId, versionId, null);
        }

        public void UploadFile(string bookId, string bookVersionId, string fileName, Stream fileStream)
        {
            CommunicationInfo commInfo = m_uriCache.GetCommunicationInfoForMethod();
            Uri uri = SetParamsToUri(commInfo.UriTemplate, bookId, bookVersionId, fileName);
            using (Stream writeStream = m_webClient.OpenWrite(uri, commInfo.Method))
            {
                fileStream.CopyTo(writeStream);
            }
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition)
        {
            return GetPageByPositionFromStart(bookId, versionId, pagePosition, null);
        }

        public string GetPageByName(string bookId, string versionId, string start)
        {
            return GetPageByName(bookId, versionId, start, null);
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end)
        {
            return GetPagesByName(bookId, versionId, start, end, null);
        }

        public Stream GetPageList(string bookId, string versionId, string xslPath)
        {
            CommunicationInfo commInfo = m_uriCache.GetCommunicationInfoForMethod();
            return m_webClient.OpenRead(GetCompleteUri(commInfo, xslPath, bookId, versionId));
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition, string xslPath)
        {
            CommunicationInfo commInfo = m_uriCache.GetCommunicationInfoForMethod();
            return m_webClient.DownloadString(GetCompleteUri(commInfo, xslPath, bookId, versionId, pagePosition));
        }

        public string GetPageByName(string bookId, string versionId, string start, string xslPath)
        {
            CommunicationInfo commInfo = m_uriCache.GetCommunicationInfoForMethod();
            return m_webClient.DownloadString(GetCompleteUri(commInfo, xslPath, bookId, versionId, start));
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end, string xslPath)
        {
            CommunicationInfo commInfo = m_uriCache.GetCommunicationInfoForMethod();
            return m_webClient.DownloadString(GetCompleteUri(commInfo, xslPath, bookId, versionId, start, end));
        }
    }
}