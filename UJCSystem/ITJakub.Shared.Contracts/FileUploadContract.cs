using System.IO;
using System.ServiceModel;

namespace ITJakub.Shared.Contracts
{
    [MessageContract]
    public class FileUploadContract
    {
        [MessageHeader]
        public string BookId { get; set; }

        [MessageHeader]
        public string BookVersionid { get; set; }

        [MessageHeader]
        public string FileName { get; set; }

        [MessageBodyMember]
        public Stream DataStream { get; set; }
    }
}