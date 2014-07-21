using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{

    [DataContract]
    public class SynchronizedObjectDetails
    {
        [DataMember]
        public User Author { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public SynchronizedObject SynchronizedObject { get; set; }
    }
}