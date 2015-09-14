using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class GroupDetailContract: GroupContract
    {
        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public UserContract CreatedBy { get; set; }

        [DataMember]
        public IList<UserContract> Members { get; set; }
    }
}