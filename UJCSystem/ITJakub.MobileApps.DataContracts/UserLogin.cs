using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class UserLogin
    {
        [DataMember]
        public  AuthenticationProviders AuthenticationProvider { get; set; }

        [DataMember]
        public string AccessToken { get; set; }

        [DataMember]
        public string Email { get; set; }
    
    }
}