using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class GroupDetails
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public UserDetails Author { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public TaskDetails Task { get; set; }

        [DataMember]
        public Group Group { get; set; }

        [DataMember]
        public IEnumerable<UserDetails> Members { get; set; }
    }
}