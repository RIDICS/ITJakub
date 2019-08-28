using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.CardFile.Core.DataContractEntities;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.CardFile.Core
{
    public class CardFilesClient
    {
        private readonly CardFilesRestClient m_client;
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<CardFilesClient>();

        public CardFilesClient(CardFilesRestClient client)
        {
            m_client = client;
        }

        public files GetFiles()
        {
            try
            {
                var result = m_client.Get<files>("files");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public buckets GetBucketsByHeadword(string fileId, string headword)
        {
            try
            {
                var result = m_client.Get<buckets>($"files/{fileId}/buckets?heslo={headword}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public buckets GetBuckets(string fileId)
        {
            try
            {
                var result = m_client.Get<buckets>($"files/{fileId}/buckets");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public buckets GetCardsFromBucket(string fileId, string bucketId)
        {
            try
            {
                var result = m_client.Get<buckets>($"files/{fileId}/buckets/{bucketId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public card GetCardFromBucket(string fileId, string bucketId, string cardId)
        {
            try
            {
                var result = m_client.Get<card>($"files/{fileId}/buckets/{bucketId}/cards/{cardId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetImageForCard(string fileId, string bucketId, string cardId, string imageId, string imageSize)
        {
            try
            {
                var result = m_client.GetStream($"files/{fileId}/buckets/{bucketId}/cards/{cardId}/images/{imageId}?size={imageSize}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
    }
}
