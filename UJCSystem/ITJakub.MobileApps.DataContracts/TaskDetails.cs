using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class TaskDetails
    {

        [DataMember]
        public User Author;

        [DataMember]
        public DateTime CreateTime;

        [DataMember]
        public Task m_task;
    }

}