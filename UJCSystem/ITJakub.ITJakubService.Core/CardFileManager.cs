using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ITJakub.CardFile.Core;
using ITJakub.CardFile.Core.DataContractEntities;
using ITJakub.ITJakubService.DataContracts;
using Jewelery;

namespace ITJakub.ITJakubService.Core
{
    public class CardFileManager
    {
        private readonly CardFilesCommunicationManager m_cardFileClient;

        public CardFileManager()
        {
            m_cardFileClient = new CardFilesCommunicationManager(); //TODO load from container
        }

        public IList<CardFileContract> GetCardFiles()
        {
            var cardFiles = m_cardFileClient.GetFiles();
            return Mapper.Map<file[], IList<CardFileContract>>(cardFiles.file);
        }

        public IList<BucketContract> GetBuckets(string cardFileId)
        {
            var buckets = m_cardFileClient.GetBuckets(cardFileId);
            return Mapper.Map<bucket[], IList<BucketContract>>(buckets.bucket);
        }

        public IList<BucketContract> GetBucketsByHeadword(string cardFileId, string headword)
        {
            var buckets = m_cardFileClient.GetBucketsByHeadword(cardFileId, headword);
            return Mapper.Map<bucket[], IList<BucketContract>>(buckets.bucket);
        }

        public IList<CardContract> GetCards(string cardFileId, string bucketId)
        {
            var buckets = m_cardFileClient.GetCardsFromBucket(cardFileId, bucketId);
            var bucketContracts = Mapper.Map<bucket[], IList<BucketContract>>(buckets.bucket);
            var cards = bucketContracts.First().Cards;
            return cards;
        }

        public CardContract GetCard(string cardFileId, string bucketId, string cardId)
        {
            var card = m_cardFileClient.GetCardFromBucket(cardFileId, bucketId,cardId);
            return Mapper.Map<card, CardContract>(card); ;
        }

        public Stream GetImage(string cardFileId, string bucketId, string cardId, string imageId, ImageSizeEnum imageSize)
        {
            return m_cardFileClient.GetImageForCard(cardFileId, bucketId, cardId, imageId, imageSize.GetStringValue());
        }
    }
}