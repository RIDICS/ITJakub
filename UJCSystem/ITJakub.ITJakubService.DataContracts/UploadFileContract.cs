using System.IO;
using System.ServiceModel;

namespace ITJakub.ITJakubService.DataContracts
{
    [MessageContract]
    public class UploadFileContract
    {
        [MessageHeader]
        public string ChangeMessage { get; set; }

        [MessageBodyMember]
        public Stream Data { get; set; }
    }
}