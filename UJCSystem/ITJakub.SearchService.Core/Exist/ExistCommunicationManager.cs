using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using log4net;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistCommunicationManager : IExistCommunicationManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ExistClient m_existClient;
        private readonly UriCache m_uriCache;

        public ExistCommunicationManager(UriCache uriCache, ExistClient existClient)
        {
            m_uriCache = uriCache;
            m_existClient = existClient;
        }

        public void UploadBookFile(string bookId, string fileName, Stream dataStream)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var uri = SetParamsToUri(commInfo.UriTemplate, bookId, fileName);
            var content = new StreamContent(dataStream);

            try
            {
                m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Wait();
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public void UploadVersionFile(string bookId, string bookVersionId, string fileName, Stream dataStream)
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start upload file '{0}' of book '{1}' and version '{2}'", fileName, bookId,
                    bookVersionId);

            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var uri = SetParamsToUri(commInfo.UriTemplate, bookId, bookVersionId, fileName);
            var content = new StreamContent(dataStream);

            try
            {
                m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Wait();
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("End upload file '{0}' of book '{1}' and version '{2}'", fileName, bookId,
                    bookVersionId);
        }

        public void UploadSharedFile(string fileName, Stream dataStream)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = SetParamsToUri(commInfo.UriTemplate, fileName);
            var content = new StreamContent(dataStream);

            try
            {
                m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Wait();
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public void UploadBibliographyFile(string bookId, string bookVersionId, string fileName, Stream dataStream)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = SetParamsToUri(commInfo.UriTemplate, bookId, bookVersionId, fileName);
            var content = new StreamContent(dataStream);

            try
            {
                m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Wait();
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
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

        public string GetBookEditionNote(string bookId, string versionId, string outputFormat)
        {
            return GetBookEditionNote(bookId, versionId, outputFormat, null);
        }

        public string ListSearchEditionsResults(string serializedSearchCriteria)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, null, serializedSearchCriteria);

            try
            {
                var result = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return result;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string ListSearchDictionariesResults(string serializedSearchCriteria)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, null, serializedSearchCriteria);

            try
            {
                var result = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return result;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public int ListSearchDictionariesResultsCount(string serializedSearchCriteria)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, null, serializedSearchCriteria);

            try
            {
                var result = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return int.Parse(result);
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public int GetSearchCriteriaResultsCount(string serializedSearchCriterias)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, null, serializedSearchCriterias);

            try
            {
                var result = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return int.Parse(result);
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string GetSearchEditionsPageList(string serializedSearchCriteria)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, null, serializedSearchCriteria);

            try
            {
                var result = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return result;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string GetEditionPageFromSearch(string serializedSearchCriteria, string bookId, string versionId, string pageXmlId, string outputFormat)
        {
            return GetEditionPageFromSearch(serializedSearchCriteria, bookId, versionId, pageXmlId, outputFormat, null);
        }

        public string GetSearchCorpus(string serializedSearchCriteria)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, null, serializedSearchCriteria);

            try
            {
                var result = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return result;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public int GetSearchCorpusCount(string serializedSearchCriteria)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, null, serializedSearchCriteria);

            try
            {
                var result = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return int.Parse(result);
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string GetBookEditionNote(string bookId, string versionId, string outputFormat, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, xslPath, bookId, versionId, outputFormat);

            try
            {
                var noteText = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return noteText;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition, string outputFormat, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, xslPath, bookId, versionId, pagePosition, outputFormat);

            try
            {
                return m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string GetPageByName(string bookId, string versionId, string start, string outputFormat, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, xslPath, bookId, versionId, start, outputFormat);

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start HTTPclient get page name '{0}' of book '{1}' and version '{2}'", start, bookId, versionId);

            try
            {
                var pageText = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;

                if (m_log.IsDebugEnabled)
                    m_log.DebugFormat("End HTTPclient get page name '{0}' of book '{1}' and version '{2}'", start, bookId, versionId);

                return pageText;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end, string outputFormat, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, xslPath, bookId, versionId, start, end, outputFormat);

            try
            {
                return m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string GetPageByXmlId(string bookId, string versionId, string pageXmlId, string outputFormat, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, xslPath, bookId, versionId, pageXmlId, outputFormat);

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start HTTPclient get page xmlId '{0}' of book '{1}' and version '{2}' and outputFormat '{3}'", pageXmlId, bookId,
                    versionId, outputFormat);

            try
            {
                var pageText = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                if (m_log.IsDebugEnabled)
                    m_log.DebugFormat("End HTTPclient get page xmlId '{0}' of book '{1}' and version '{2}' and outputFormat '{3}'", pageXmlId, bookId, versionId,
                        outputFormat);

                return pageText;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId, string outputFormat, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, xslPath, bookId, versionId, xmlEntryId, outputFormat);

            try
            {
                var entryResult = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return entryResult;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string GetDictionaryEntryFromSearch(string serializedSearchCriteria, string bookId, string versionId, string xmlEntryId, string outputFormat,
            string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, xslPath, serializedSearchCriteria, bookId, versionId, xmlEntryId, outputFormat);

            try
            {
                var entryResult = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return entryResult;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        public string GetEditionPageFromSearch(string serializedSearchCriteria, string bookId, string versionId, string pageXmlId, string outputFormat,
            string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();
            var uri = GetCompleteUri(commInfo, null);
            var content = GetContentKeyValuePairs(commInfo, xslPath, serializedSearchCriteria, bookId, versionId, pageXmlId, outputFormat);

            try
            {
                var pageText = m_existClient.SendRequestGetResponseAsStringAsync(commInfo, uri, content).Result;
                return pageText;
            }
            catch (AggregateException exception)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error($"{GetCurrentMethodName()} failed with {exception.GetType().Name}", exception);
                throw;
            }
        }

        #region Helpers

        private static Uri SetParamsToUri(string uriTemplate, params object[] args)
        {
            var uriString = string.Format(uriTemplate, args);
            return new Uri(uriString);
        }

        private static string SetParamsToStringTemplate(string template, params object[] args)
        {
            return string.IsNullOrEmpty(template) ? string.Empty : string.Format(template, args);
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

        private static StringContent GetContentKeyValuePairs(CommunicationInfo commInfo, string xslPath, params object[] args)
        {
            var contentTemplate = commInfo.ContentTemplate;
            if (!string.IsNullOrEmpty(xslPath))
            {
                contentTemplate = AddXslParam(contentTemplate, xslPath);
            }
            var contentPairString = SetParamsToStringTemplate(contentTemplate, args);
            return new StringContent(contentPairString, Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        private string GetCurrentMethodName([CallerMemberName] string methodName = null)
        {
            return methodName;
        }

        #endregion
    }
}