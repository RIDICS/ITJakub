using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.RequestObjects
{
    [DataContract]
    public class Institution
    {
        [DataMember]
        public string Name;

        //TODO add adress

        [DataMember]
        public User Principal;
    }
}