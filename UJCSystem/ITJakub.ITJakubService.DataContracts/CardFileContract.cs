using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class CardFileContract
    {
        [DataMember]
        public string Id { get; set; }
    }
}