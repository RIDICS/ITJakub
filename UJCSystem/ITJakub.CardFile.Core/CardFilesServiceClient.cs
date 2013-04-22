using System;
using System.IO;
using System.ServiceModel;
using Ujc.Naki.CardFile.Core.DataContractEntities;

namespace Ujc.Naki.CardFile.Core
{
    public sealed class CardFilesServiceClient : ClientBase<ICardFilesService>, ICardFilesService
    {
        public files GetFiles()
        {
            return Channel.GetFiles();
        }

        public buckets GetBuckets(string fileId, string heslo)
        {
            return Channel.GetBuckets(fileId, heslo);
        }

        public buckets GetCardsFromBucket(string fileId, string bucketId)
        {
            return Channel.GetCardsFromBucket(fileId, bucketId);
        }

        public card GetCardFromBucket(string fileId, string bucketId, string cardId)
        {
            return Channel.GetCardFromBucket(fileId, bucketId, cardId);
        }

        public Stream GetImageForCard(string fileId, string bucketId, string cardId, string imageId, string imageSize)
        {
            return Channel.GetImageForCard(fileId, bucketId, cardId, imageId, imageSize);
        }
    }
}