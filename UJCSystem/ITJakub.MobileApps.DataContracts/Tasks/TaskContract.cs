using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Tasks
{
    [DataContract]
    [KnownType(typeof(TaskDetailContract))]
    [KnownType(typeof(TaskDataContract))]
    public class TaskContract
    {
        [DataMember]
        public long Id { get; set; }
        
        [DataMember]
        public int ApplicationId { get; set; }
    }

    [DataContract]
    public class TaskDataContract : TaskContract
    {
        [DataMember]
        public string Data { get; set; }
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
