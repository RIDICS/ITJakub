using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class CardContract
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public string Headword { get; set; }

        [DataMember]
        public IList<ImageContract> Images { get; set; }

        [DataMember]
        public string Warning { get; set; }

        [DataMember]
        public string Note { get; set; }
    }
}