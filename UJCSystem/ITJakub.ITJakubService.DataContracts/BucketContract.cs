using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class BucketContract
    {
        [DataMember]
        public string Id { get; set; }
    }
}