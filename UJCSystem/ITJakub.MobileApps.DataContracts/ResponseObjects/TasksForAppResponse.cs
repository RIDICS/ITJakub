using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.ResponseObjects
{
    [DataContract]
    public class TasksForAppResponse
    {
        [DataMember]
        public IEnumerable<Task> Tasks;
    }


    [DataContract]
    public class Task
    {
        [DataMember]
        public DateTime CreateTime;

        [DataMember]
        public string Data;
    }

}