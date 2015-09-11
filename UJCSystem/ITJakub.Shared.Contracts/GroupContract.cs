using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class GroupContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public UserContract CreatedBy { get; set; }

        [DataMember]
        public IList<UserContract> Members { get; set; }
    }
}