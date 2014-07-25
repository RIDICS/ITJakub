﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class Group
    {
        [DataMember]
        public IEnumerable<UserBasicDetails> Members { get; set; }
    }
}