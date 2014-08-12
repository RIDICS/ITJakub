using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class UserLogin
    {
        [DataMember]
        public  AuthProvidersContract AuthProviderContract { get; set; }

        [DataMember]
        public string AuthenticationToken { get; set; }

        [DataMember]
        public string Email { get; set; }
    
    }
}