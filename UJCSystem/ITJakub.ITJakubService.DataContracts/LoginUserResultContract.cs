using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class LoginUserResultContract
    {
        [DataMember]
        public bool Successfull { get; set; }

        [DataMember]
        public string CommunicationToken { get; set; }
    }
}