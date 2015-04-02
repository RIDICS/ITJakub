using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistClient : IExistClient
    {
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

        public async Task<Stream> GetPageListAsync(string bookId, string versionId)
        {
            return await GetPageListAsync(bookId, versionId, null);
        }

        public async Task UploadVersionFileAsync(string bookId, string bookVersionId, string fileName, Stream dataStream)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var uri = SetParamsToUri(commInfo.UriTemplate, bookId, bookVersionId, fileName);
            await m_httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(commInfo.Method), uri)
            {
                Content = new StreamContent(dataStream)
            });
        }

        public async Task UploadBookFileAsync(string bookId, string fileName, Stream dataStream)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var uri = SetParamsToUri(commInfo.UriTemplate, bookId, fileName);
            await m_httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(commInfo.Method), uri)
            {
                Content = new StreamContent(dataStream)
            });
        }

        public async Task UploadSharedFileAsync(string fileName, Stream dataStream)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var uri = SetParamsToUri(commInfo.UriTemplate, fileName);
            await m_httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(commInfo.Method), uri)
            {
                Content = new StreamContent(dataStream)
            });
        }

        public async Task<string> GetPageByPositionFromStart(string bookId, string versionId, int pagePosition)
        {
            return await GetPageByPositionFromStart(bookId, versionId, pagePosition, null);
        }

        public async Task<string> GetPageByName(string bookId, string versionId, string start)
        {
            return await GetPageByName(bookId, versionId, start, null);
        }

        public async Task<string> GetPagesByName(string bookId, string versionId, string start, string end)
        {
            return await GetPagesByName(bookId, versionId, start, end, null);
        }

        public async Task<Stream> GetPageListAsync(string bookId, string versionId, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId);
            var response = await m_httpClient.GetAsync(completeUri);
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<string> GetPageByPositionFromStart(string bookId, string versionId, int pagePosition,
            string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var response = await m_httpClient.GetStringAsync(GetCompleteUri(commInfo, xslPath, bookId, versionId, pagePosition));
            return response;
        }

        public async Task<string> GetPageByName(string bookId, string versionId, string start, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            var completeUri = GetCompleteUri(commInfo, xslPath, bookId, versionId, start);
            return await m_httpClient.GetStringAsync(completeUri);
        }

        public async Task<string> GetPagesByName(string bookId, string versionId, string start, string end, string xslPath)
        {
            var commInfo = m_uriCache.GetCommunicationInfoForMethod();

            return await m_httpClient.GetStringAsync(GetCompleteUri(commInfo, xslPath, bookId, versionId, start, end));
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