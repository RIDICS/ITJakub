using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Lemmatization.Shared.Contracts
{
    [DataContract]
    public class TokenCharacteristicContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string MorphologicalCharacteristic { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public IList<CanonicalFormContract> CanonicalFormList { get; set; } 
    }
}