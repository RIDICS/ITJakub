using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts
{
    [DataContract]
    public class ImageContract
    {
        [DataMember]
        public string Id { get; set; }
    }
}