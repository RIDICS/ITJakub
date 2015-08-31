using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts
{
    [DataContract]
    public class CreateUserResultContract
    {
        [DataMember]
        public bool Successfull { get; set; }
    }
}