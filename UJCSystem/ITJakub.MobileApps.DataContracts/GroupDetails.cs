using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class GroupDetails
    {

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long AuthorId { get; set; }

        [DataMember]
        public DateTime CreateTime;

        [DataMember] 
        public Group Group;

    }
}