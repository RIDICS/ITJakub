using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.ResponseObjects
{
    [DataContract]
    public class SynchronizedObjectsResponse
    {
        [DataMember]
        public IEnumerable<SynchronizedObjectDetails> SyncedObjects;
    }

    [DataContract]
    public class SynchronizedObjectDetails
    {
        [DataMember]
        public UserDetailsResponse Author;

        [DataMember] 
        public DateTime CreateTime;

        [DataMember]
        public string Data;
    }
}