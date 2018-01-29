using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.SearchService.DataContracts.Contracts
{
    [MessageContract]
    [KnownType(typeof (BookResourceUploadContract))]
    public class ResourceUploadContract
    {
        [MessageHeader]
        public string FileName { get; set; }

        [MessageHeader]
        public ResourceType ResourceType { get; set; }

        [MessageBodyMember]
        public Stream DataStream { get; set; }
    }

    [MessageContract]
    [KnownType(typeof(VersionResourceUploadContract))]
    public class BookResourceUploadContract : ResourceUploadContract
    {
        [MessageHeader]
        public string BookId { get; set; }
    }

    [MessageContract]
    public class VersionResourceUploadContract : BookResourceUploadContract
    {
        [MessageHeader]
        public string BookVersionId { get; set; }
    }
}