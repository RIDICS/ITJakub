using System;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts.Favorite
{
    [DataContract]
    public class FavoriteLabelContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public DateTime? LastUseTime { get; set; }
    }
}