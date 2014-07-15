using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.RequestObjects
{
    [DataContract]
    public class Group
    {
        [DataMember]
        public string AppGUID;

        [DataMember]
        public IEnumerable<User> Users;
    }
}