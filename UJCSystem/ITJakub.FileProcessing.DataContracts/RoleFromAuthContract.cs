using System.Runtime.Serialization;

namespace ITJakub.FileProcessing.DataContracts
{
    [DataContract]
    public class RoleFromAuthContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}