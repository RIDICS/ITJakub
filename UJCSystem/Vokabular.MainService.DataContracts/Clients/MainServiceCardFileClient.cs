using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts.CardFile;
using Vokabular.RestClient;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceCardFileClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceCardFileClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public List<CardFileContract> GetCardFiles()
        {
            try
            {
                var result = m_client.Get<List<CardFileContract>>("cardfile");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<BucketShortContract> GetBuckets(string cardFileId, string headword = null)
        {
            try
            {
                var url = UrlQueryBuilder.Create($"cardfile/{cardFileId}/bucket")
                    .AddParameter("headword", headword)
                    .ToQuery();
                var result = m_client.Get<List<BucketShortContract>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CardContract> GetCards(string cardFileId, string bucketId)
        {
            try
            {
                var result = m_client.Get<List<CardContract>>($"cardfile/{cardFileId}/bucket/{bucketId}/card");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CardShortContract> GetCardsShort(string cardFileId, string bucketId)
        {
            try
            {
                var result = m_client.Get<List<CardShortContract>>($"cardfile/{cardFileId}/bucket/{bucketId}/card/short");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public CardContract GetCard(string cardFileId, string bucketId, string cardId)
        {
            try
            {
                var result = m_client.Get<CardContract>($"cardfile/{cardFileId}/bucket/{bucketId}/card/{cardId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetCardImage(string cardFileId, string bucketId, string cardId, string imageId,
            CardImageSizeEnumContract imageSize)
        {
            try
            {
                var result = m_client.GetStream($"cardfile/{cardFileId}/bucket/{bucketId}/card/{cardId}/image/{imageId}?imageSize={imageSize}");
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
