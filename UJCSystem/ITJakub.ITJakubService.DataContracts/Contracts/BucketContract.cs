using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts
{
    [DataContract]
    public class BucketContract
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int CardsCount { get; set; }

        [DataMember]
        public IList<CardContract> Cards { get; set; }
    }
}