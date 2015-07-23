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
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly HttpClient m_httpClient;
        private readonly UriCache m_uriCache;

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
                m_log.DebugFormat("Start upload file '{0}' of book '{1}' and version '{2}'", fileName, bookId,
                    bookVersionId);

            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var uri = SetParamsToUri(commInfo.UriTemplate, bookId, bookVersionId, fileName);
            Task.Run(() => m_httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(commInfo.Method), uri)
            {
                Content = new StreamContent(dataStream)
            })).Wait();

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("End upload file '{0}' of book '{1}' and version '{2}'", fileName, bookId,
                    bookVersionId);
        }

        public void UploadBookFile(string bookId, string fileName, Stream dataStream)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var uri = SetParamsToUri(commInfo.UriTemplate, bookId, fileName);
            Task.Run(() => m_httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(commInfo.Method), uri)
            {
                Content = new StreamContent(dataStream)
            })).Wait();
        }

        public void UploadSharedFile(string fileName, Stream dataStream)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = SetParamsToUri(commInfo.UriTemplate, fileName);
            Task.Run(() => m_httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(commInfo.Method), uri)
            {
                Content = new StreamContent(dataStream)
            })).Wait();
        }

        public string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId, string outputFormat)
        {
            return GetDictionaryEntryByXmlId(bookId, versionId, xmlEntryId, outputFormat, null);
        }

        public string GetDictionaryEntryFromSearch(string serializedSearchCriteria, string bookId, string versionId, string xmlEntryId,
            string outputFormat)
        {
            return GetDictionaryEntryFromSearch(serializedSearchCriteria, bookId, versionId, xmlEntryId, outputFormat, null);
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition, string outputFormat)
        {
            return GetPageByPositionFromStart(bookId, versionId, pagePosition, outputFormat, null);
        }

        public string GetPageByName(string bookId, string versionId, string start, string outputFormat)
        {
            return GetPageByName(bookId, versionId, start, outputFormat, null);
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end, string outputFormat)
        {
            return GetPagesByName(bookId, versionId, start, end, outputFormat, null);
        }

        public string GetPageByXmlId(string bookId, string versionId, string pageXmlId, string outputFormat)
        {
            return GetPageByXmlId(bookId, versionId, pageXmlId, outputFormat, null);
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition, string outputFormat,
            string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId, pagePosition, outputFormat);
            return Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;
        }

        public string GetPageByName(string bookId, string versionId, string start, string outputFormat, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId, start, outputFormat);
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start HTTPclient get page name '{0}' of book '{1}' and version '{2}'", start, bookId,
                    versionId);
            var pageText = Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("End HTTPclient get page name '{0}' of book '{1}' and version '{2}'", start, bookId,
                    versionId);
            return pageText;
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end, string outputFormat,
            string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId, start, end, outputFormat);
            return Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;
        }

        public string GetPageByXmlId(string bookId, string versionId, string pageXmlId, string outputFormat,
            string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId, pageXmlId, outputFormat);

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start HTTPclient get page xmlId '{0}' of book '{1}' and version '{2}' and outputFormat '{3}'", pageXmlId, bookId, versionId, outputFormat);
            
            var pageText = Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("End HTTPclient get page xmlId '{0}' of book '{1}' and version '{2}' and outputFormat '{3}'", pageXmlId, bookId, versionId, outputFormat);

            return pageText;
        }

        public string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId, string outputFormat,
            string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId, xmlEntryId, outputFormat);
            var entryResult = Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;

            return entryResult;
        }

        public string GetDictionaryEntryFromSearch(string serializedSearchCriteria, string bookId, string versionId, string xmlEntryId, string outputFormat, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, xslPath, serializedSearchCriteria, bookId, versionId, xmlEntryId, outputFormat);
            var entryResult = Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;

            return entryResult;
        }

        public string ListSearchEditionsResults(string serializedSearchCriteria)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, null, serializedSearchCriteria);
            var result = Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;

            return result;
        }

        public string ListSearchDictionariesResults(string serializedSearchCriteria)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, null, serializedSearchCriteria);
            var result = Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;

            return result;
        }

        public int ListSearchDictionariesResultsCount(string serializedSearchCriteria)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, null, serializedSearchCriteria);
            var result = Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;

            return int.Parse(result);
        }
        public int GetSearchCriteriaResultsCount(string serializedSearchCriterias)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var completeUri = GetCompleteUri(commInfo, null, serializedSearchCriterias);
            var result = Task.Run(() => m_httpClient.GetStringAsync(completeUri)).Result;

            return int.Parse(result);
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

        
    }
}