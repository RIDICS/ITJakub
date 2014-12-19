using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts

{
    [DataContract]
    public class AuthorContract
    {
        [DataMember]
        public string Name { get; set; }
    }
}