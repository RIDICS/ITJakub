using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.FileProcessing.DataContracts
{
    [DataContract]
    public class PermissionFromAuthContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IList<int> RoleExternalIds { get; set; }
    }
}
