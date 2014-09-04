using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Tasks
{
    [DataContract]
    [KnownType(typeof(TaskDetailContract))]
    public class TaskContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public int ApplicationId { get; set; }
    }

    [DataContract]
    public class TaskDetailContract : TaskContract
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public UserDetailContract Author { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }
    }
}
