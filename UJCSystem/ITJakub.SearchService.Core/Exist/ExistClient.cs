using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using log4net;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistClient : IExistClient
    {
        private readonly HttpClient m_httpClient;
        private readonly UriCache m_uriCache;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ExistClient(UriCache uriCache, ExistConnectionSettingsSkeleton connectionSettings)
        {
            m_uriCache = uriCache;
            var clientHandler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(connectionSettings.DBUser, connectionSettings.DBPassword)
            };
            m_httpClient = new HttpClient(clientHandler);
        }

        public void UploadVersionFile(string bookId, string bookVersionId, string fileName, Stream dataStream)
        {
            if (m_log.IsDebugEnabled) 
                m_log.DebugFormat("Begin upload file '{0}' of book '{1}' and version '{2}'", fileName, bookId, bookVersionId);
           
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var uri = SetParamsToUri(commInfo.UriTemplate, bookId, bookVersionId, fileName);
            Task.Run(() => m_httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(commInfo.Method), uri)
            {
                Content = new StreamContent(dataStream)
            })); 

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("End upload file '{0}' of book '{1}' and version '{2}'", fileName, bookId, bookVersionId);
        }

        public void UploadBookFile(string bookId, string fileName, Stream dataStream)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var uri = SetParamsToUri(commInfo.UriTemplate, bookId, fileName);
            Task.Run(() => m_httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(commInfo.Method), uri)
            {
                Content = new StreamContent(dataStream)
            })); 
        }

        public void UploadSharedFile(string fileName, Stream dataStream)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = SetParamsToUri(commInfo.UriTemplate, fileName);
            Task.Run(() => m_httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(commInfo.Method), uri)
            {
                Content = new StreamContent(dataStream)
            }));
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

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition,
            string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId, pagePosition);
            return Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;
        }

        public string GetPageByName(string bookId, string versionId, string start, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId, start);
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start HTTPclient get page name '{0}' of book '{1}' and version '{2}'", start, bookId, versionId);
            string pageText = Task.Run(()=>m_httpClient.GetStringAsync(completeUri)).Result;
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("End HTTPclient get page name '{0}' of book '{1}' and version '{2}'", start, bookId, versionId);
            return pageText;
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId, start, end);
            return Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;
        }


        #region Helpers


        private static Uri SetParamsToUri(string uriTemplate, params object[] args)
        {
            var uriString = string.Format(uriTemplate, args);
            return new Uri(uriString);
        }

        private static string AddXslParam(string uriTemplate, string xslPath)
        {
            return string.Format("{0}&_xsl={1}", uriTemplate, xslPath); //TODO add check for "?"
        }

        private static Uri GetCompleteUri(CommunicationInfo commInfo, string xslPath, params object[] args)
        {
            var uriTemplate = commInfo.UriTemplate;
            if (!string.IsNullOrEmpty(xslPath))
            {
                uriTemplate = AddXslParam(uriTemplate, xslPath);
            }
            return SetParamsToUri(uriTemplate, args);
        }

        #endregion

        public string GetPageByXmlId(string bookId, string versionId, string pageXmlId, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId, pageXmlId);
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start HTTPclient get page xmlId '{0}' of book '{1}' and version '{2}'", pageXmlId, bookId, versionId);
            string pageText = Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("End HTTPclient get page xmlId '{0}' of book '{1}' and version '{2}'", pageXmlId, bookId, versionId);
            return pageText;
        }


        public string GetPageByXmlId(string bookId, string versionId, string pageXmlId)
        {
            return GetPageByXmlId(bookId, versionId, pageXmlId, null);
        }
    }
}