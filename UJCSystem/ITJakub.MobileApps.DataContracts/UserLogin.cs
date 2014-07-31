using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class UserLogin
    {
        [DataMember]
        public  AuthenticationProviders AuthenticationProvider { get; set; }


        /// <summary>
        /// For third party accessToken, for own service PasswordHash
        /// </summary>
        [DataMember]
        public string AuthenticationToken { get; set; }

        [DataMember]
        public string Email { get; set; }
    
    }
}