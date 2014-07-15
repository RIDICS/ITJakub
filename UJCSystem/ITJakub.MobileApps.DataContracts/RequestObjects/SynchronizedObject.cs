using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.RequestObjects
{
    [DataContract]
    public class SynchronizedObject
    {
        [DataMember]
        public string Data;
    }
}