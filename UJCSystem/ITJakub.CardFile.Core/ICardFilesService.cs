using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Ujc.Naki.CardFile.Core.DataContractEntities;

namespace Ujc.Naki.CardFile.Core
{
    [ServiceContract]
    [XmlSerializerFormat]
    public interface ICardFilesService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/files", Method = "GET", ResponseFormat = WebMessageFormat.Xml)]
        files GetFiles();

        [OperationContract]
        [WebInvoke(UriTemplate = "/files/{fileId}/buckets?heslo={heslo}", Method = "GET", ResponseFormat = WebMessageFormat.Xml)]
        buckets GetBuckets(string fileId, string heslo);


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
}