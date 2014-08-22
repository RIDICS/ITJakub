using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Tasks
{
    [DataContract]
    public class TaskContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public UserDetailContract Author { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public string Data { get; set; }
    }
}
