using System.IO;
using System.ServiceModel;

namespace ITJakub.ITJakubService.DataContracts
{
    [MessageContract]
    public class UploadFileContract
    {
        [MessageHeader]
        public string SessionId { get; set; }

        [MessageHeader]
        public string Name { get; set; }

        [MessageBodyMember]
        public Stream Data { get; set; }
    }
}