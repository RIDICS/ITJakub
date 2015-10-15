using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    [KnownType(typeof(GroupDetailContract))]
    public class GroupContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}