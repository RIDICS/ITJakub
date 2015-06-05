using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class ImageContract
    {
        [DataMember]
        public string Id { get; set; }
    }
}