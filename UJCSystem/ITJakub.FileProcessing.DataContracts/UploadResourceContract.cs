using System.IO;
using System.ServiceModel;

namespace ITJakub.FileProcessing.DataContracts
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