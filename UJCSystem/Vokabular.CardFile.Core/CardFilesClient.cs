using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Vokabular.RestClient;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.CardFile.Core
{
    public class CardFilesClient : FullRestClientBase
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<CardFilesClient>();

        public CardFilesClient(Uri baseAddress, string username, string password) : base(baseAddress, true)
        {
            var networkCredentials = new NetworkCredential(username, password);
            var credCache = new CredentialCache();
            credCache.Add(baseAddress, "Digest", networkCredentials);

            HttpClientHandler.Credentials = credCache;

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {
        }

        public string GetFiles()
        {
            try
            {
                var result = GetString("files");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
    }
}
