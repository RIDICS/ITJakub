using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class Group
    {
        [DataMember]
        public Task Task { get; set; }
        
        [DataMember]
        public IEnumerable<User> Members { get; set; }

    
    }
}