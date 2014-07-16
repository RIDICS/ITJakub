using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class Institution
    {
        [DataMember]
        public string Name;

        //users working for institution (principal, teachers)
        [DataMember]
        public User Principal;
    }
}