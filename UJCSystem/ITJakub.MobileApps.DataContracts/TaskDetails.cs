using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class TaskDetails
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public UserDetails Author { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public Task Task { get; set; }
    }
}