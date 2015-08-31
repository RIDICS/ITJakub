using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts
{
    [DataContract]
    public class AuthorDetailContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}