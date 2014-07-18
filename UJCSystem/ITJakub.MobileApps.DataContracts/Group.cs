using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class Group
    {
        [DataMember]
        public long TaskId { get; set; }
        
        [DataMember]
        public IEnumerable<User> Users { get; set; }

    
    }
}