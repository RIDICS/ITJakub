using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Vokabular.CardFile.Core.DataContractEntities;
using Vokabular.RestClient;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.CardFile.Core
{
    public class CardFilesClient : FullRestClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<CardFilesClient>();

        public CardFilesClient(Uri baseAddress, string username, string password) : base(new ServiceCommunicationConfiguration{ Url = baseAddress}, true)
        {
            var networkCredentials = new NetworkCredential(username, password);
            var credCache = new CredentialCache();
            credCache.Add(baseAddress, "Digest", networkCredentials);

            HttpClientHandler.Credentials = credCache;

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            DeserializationType = DeserializationType.Xml;
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {
        }

        public files GetFiles()
        {
            try
            {
                var result = Get<files>("files");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public buckets GetBucketsByHeadword(string fileId, string headword)
        {
            try
            {
                var result = Get<buckets>($"files/{fileId}/buckets?heslo={headword}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public buckets GetBuckets(string fileId)
        {
            try
            {
                var result = Get<buckets>($"files/{fileId}/buckets");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public buckets GetCardsFromBucket(string fileId, string bucketId)
        {
            try
            {
                var result = Get<buckets>($"files/{fileId}/buckets/{bucketId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public card GetCardFromBucket(string fileId, string bucketId, string cardId)
        {
            try
            {
                var result = Get<card>($"files/{fileId}/buckets/{bucketId}/cards/{cardId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetImageForCard(string fileId, string bucketId, string cardId, string imageId, string imageSize)
        {
            try
            {
                var result = GetStream($"files/{fileId}/buckets/{bucketId}/cards/{cardId}/images/{imageId}?size={imageSize}");
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
