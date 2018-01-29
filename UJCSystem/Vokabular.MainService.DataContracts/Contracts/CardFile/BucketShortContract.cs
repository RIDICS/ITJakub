using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vokabular.MainService.DataContracts.Contracts.CardFile
{
    [DataContract]
    public class BucketShortContract
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int CardsCount { get; set; }

        [DataMember]
        public IList<CardShortContract> Cards { get; set; }
    }
}