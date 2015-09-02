using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts
{
    [DataContract]
    public class CardContract
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public IEnumerable<string> Headwords { get; set; }

        [DataMember]
        public IEnumerable<ImageContract> Images { get; set; }

        [DataMember]
        public IEnumerable<string> Warnings { get; set; }

        [DataMember]
        public IEnumerable<string> Notes { get; set; }
    }
}