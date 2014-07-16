using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class InstitutionDetails
    {
        [DataMember] 
        public Institution InstitutionBaseInfo;

        //users working for institution (principal, teachers)
        [DataMember]
        public IEnumerable<UserDetails> Users;
    }
}