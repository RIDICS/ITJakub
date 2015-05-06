using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using ITJakub.CardFile.Core.DataContractEntities;

namespace ITJakub.CardFile.Core
{
    [ServiceContract]
    [XmlSerializerFormat]
    public interface ICardFilesService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/files", Method = "GET", ResponseFormat = WebMessageFormat.Xml)]
        files GetFiles();

        [OperationContract]
        [WebInvoke(UriTemplate = "/files/{fileId}/buckets?heslo={headword}", Method = "GET", ResponseFormat = WebMessageFormat.Xml)]
        buckets GetBucketsByHeadword(string fileId, string headword);

        [OperationContract]
        [WebInvoke(UriTemplate = "/files/{fileId}/buckets", Method = "GET", ResponseFormat = WebMessageFormat.Xml)]
        buckets GetBuckets(string fileId);


        [OperationContract]
        [WebInvoke(UriTemplate = "/files/{fileId}/buckets/{bucketId}", Method = "GET", ResponseFormat = WebMessageFormat.Xml)]
        buckets GetCardsFromBucket(string fileId, string bucketId);


        [OperationContract]
        [WebInvoke(UriTemplate = "/files/{fileId}/buckets/{bucketId}/cards/{cardId}", Method = "GET", ResponseFormat = WebMessageFormat.Xml)]
        card GetCardFromBucket(string fileId, string bucketId, string cardId);

        [OperationContract]
        [WebInvoke(UriTemplate = "/files/{fileId}/buckets/{bucketId}/cards/{cardId}/images/{imageId}?size={imageSize}", Method = "GET")]
        [DataContractFormat]
        Stream GetImageForCard(string fileId, string bucketId, string cardId, string imageId, string imageSize);
    }

    public sealed class CardFilesServiceClient : ClientBase<ICardFilesService>, ICardFilesService
    {
        public files GetFiles()
        {
            return Channel.GetFiles();
        }

        public buckets GetBucketsByHeadword(string fileId, string headword)
        {
            return Channel.GetBucketsByHeadword(fileId, headword);
        }
        public buckets GetBuckets(string fileId)
        {
            return Channel.GetBuckets(fileId);
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