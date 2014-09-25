using System.ServiceModel;

namespace ITJakub.ITJakubService.DataContracts
{
    [MessageContract]
    public class ProcessedFileInfoContract
    {
        [MessageBodyMember]
        public string FileGuid { get; set; }

        [MessageBodyMember]
        public string VersionId { get; set; }

        [MessageBodyMember]
        public string Name { get; set; }

        [MessageBodyMember]
        public string Author { get; set; }
    }
}