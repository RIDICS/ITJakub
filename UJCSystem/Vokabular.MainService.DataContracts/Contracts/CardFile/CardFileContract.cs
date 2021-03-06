﻿using System.Runtime.Serialization;

namespace Vokabular.MainService.DataContracts.Contracts.CardFile
{
    [DataContract]
    public class CardFileContract
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int BucketsCount { get; set; }
    }
}