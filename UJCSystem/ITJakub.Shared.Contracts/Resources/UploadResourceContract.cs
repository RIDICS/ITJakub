using System.IO;
using System.ServiceModel;

namespace ITJakub.Shared.Contracts.Resources
{
    [MessageContract]
    public class UploadResourceContract
    {
        [MessageHeader]
        public string SessionId { get; set; }

        [MessageHeader]
        public string FileName { get; set; }

        [MessageBodyMember]
        public Stream Data { get; set; }
    }
}