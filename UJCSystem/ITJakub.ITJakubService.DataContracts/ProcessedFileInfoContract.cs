using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    //TODO change members according to info in header element of xml file
    [DataContract]
    public class ProcessedFileInfoContract
    {
        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Author { get; set; }
    }
}