using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Applications
{
    [DataContract]
    public class SynchronizedObjectResponseContract
    {
        [DataMember]
        public UserDetailContract Author { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public string Data { get; set; }
    }
}