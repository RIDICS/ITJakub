using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class CreateUserResultContract
    {
        [DataMember]
        public bool Successfull { get; set; }
    }
}