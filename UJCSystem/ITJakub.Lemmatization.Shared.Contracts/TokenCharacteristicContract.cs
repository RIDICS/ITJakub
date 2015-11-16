using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Lemmatization.Shared.Contracts
{
    [DataContract]
    [KnownType(typeof(TokenCharacteristicDetailContract))]
    [KnownType(typeof(InverseTokenCharacteristicContract))]
    public class TokenCharacteristicContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string MorphologicalCharacteristic { get; set; }

        [DataMember]
        public string Description { get; set; }

    }

    [DataContract]
    public class TokenCharacteristicDetailContract : TokenCharacteristicContract
    {
        [DataMember]
        public IList<CanonicalFormTypeaheadContract> CanonicalFormList { get; set; }
    }

    [DataContract]
    public class InverseTokenCharacteristicContract : TokenCharacteristicContract
    {
        [DataMember]
        public TokenContract Token { get; set; }
    }
}