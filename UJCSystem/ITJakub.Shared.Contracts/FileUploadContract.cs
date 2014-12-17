using System.IO;
using System.ServiceModel;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.Shared.Contracts
{
    [MessageContract]
    public class FileUploadContract
    {
        [MessageHeader]
        public string BookId { get; set; }

        [MessageHeader]
        public string BookVersionId { get; set; }

        [MessageHeader]
        public string FileName { get; set; }

        [MessageHeader]
        public ResourceTypeEnum ResourceType { get; set; }

        [MessageBodyMember]
        public Stream DataStream { get; set; }
    }
}