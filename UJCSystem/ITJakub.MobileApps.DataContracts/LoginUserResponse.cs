using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class LoginUserResponse
    {
        [DataMember]
        public string CommunicationToken { get; set; }
    }
}