using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class CreateGroupResponse
    {
        [DataMember]
        public string EnterCode { get; set; }
    }
}