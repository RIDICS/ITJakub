using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.ResponseObjects
{
    [DataContract]
    public class InstitutionDetailsResponse
    {
        [DataMember]
        public string Name;

        [DataMember]
        public IEnumerable<UserDetailsResponse> Users;
    }
}