using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.CardFile.Core;
using Vokabular.CardFile.Core.DataContractEntities;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts.CardFile;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Attribute;

namespace Vokabular.MainService.Core.Managers
{
    public class CardFileManager
    {
        private readonly AuthorizationManager m_authorizationManager;
        private readonly CardFilesClient m_cardFileClient;

        public CardFileManager(AuthorizationManager authorizationManager, CommunicationProvider communicationProvider)
        {
            m_authorizationManager = authorizationManager;
            m_cardFileClient = communicationProvider.GetCardFilesClient();
        }
        
        public IList<CardFileContract> GetCardFiles()
        {
            var cardFiles = m_cardFileClient.GetFiles();
            var cardFilesContracts = Mapper.Map<file[], IList<CardFileContract>>(cardFiles.file);
            m_authorizationManager.FilterCardFileList(ref cardFilesContracts);
            return cardFilesContracts;
        }

        public IList<BucketShortContract> GetBuckets(string cardFileId)
        {
            m_authorizationManager.CheckUserCanViewCardFile(cardFileId);
            var buckets = m_cardFileClient.GetBuckets(cardFileId);
            return Mapper.Map<bucket[], IList<BucketShortContract>>(buckets.bucket);
        }

        public IList<BucketShortContract> GetBucketsByHeadword(string cardFileId, string headword)
        {
            m_authorizationManager.CheckUserCanViewCardFile(cardFileId);
            var buckets = m_cardFileClient.GetBucketsByHeadword(cardFileId, headword);
            return buckets.bucket != null
                ? Mapper.Map<bucket[], IList<BucketShortContract>>(buckets.bucket)
                : null;
        }

        public IList<CardContract> GetCards(string cardFileId, string bucketId)
        {
            m_authorizationManager.CheckUserCanViewCardFile(cardFileId);
            var buckets = m_cardFileClient.GetCardsFromBucket(cardFileId, bucketId);
            var bucketContracts = Mapper.Map<bucket[], IList<BucketContract>>(buckets.bucket);
            var cards = bucketContracts.First().Cards;
            return cards;
        }
        public IList<CardShortContract> GetCardsShort(string cardFileId, string bucketId)
        {
            m_authorizationManager.CheckUserCanViewCardFile(cardFileId);
            var buckets = m_cardFileClient.GetCardsFromBucket(cardFileId, bucketId);
            var bucketContracts = Mapper.Map<bucket[], IList<BucketShortContract>>(buckets.bucket);
            var cards = bucketContracts.First().Cards;
            return cards;
        }

        public CardContract GetCard(string cardFileId, string bucketId, string cardId)
        {
            m_authorizationManager.CheckUserCanViewCardFile(cardFileId);
            var card = m_cardFileClient.GetCardFromBucket(cardFileId, bucketId,cardId);
            return Mapper.Map<card, CardContract>(card);
        }

        public FileResultData GetImage(string cardFileId, string bucketId, string cardId, string imageId, CardImageSizeEnumContract imageSize)
        {
            m_authorizationManager.CheckUserCanViewCardFile(cardFileId);
            return m_cardFileClient.GetImageForCard(cardFileId, bucketId, cardId, imageId, imageSize.GetStringValue());
        }
    }
}