using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class GroupDetails
    {
        [DataMember]
        public User Author;

        [DataMember]
        public DateTime CreateTime;

        [DataMember] 
        public Group Group;
    }
}