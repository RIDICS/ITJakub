using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class AppTaskDetails
    {

        [DataMember]
        public User Author;

        [DataMember]
        public DateTime CreateTime;

        [DataMember]
        public AppTask AppTask;
    }

}