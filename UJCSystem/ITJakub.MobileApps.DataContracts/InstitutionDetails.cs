using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class InstitutionDetails
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IEnumerable<UserDetails> Employees { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }
    }
}