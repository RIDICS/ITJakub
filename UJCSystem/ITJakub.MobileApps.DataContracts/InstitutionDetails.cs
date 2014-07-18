using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class InstitutionDetails
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember] 
        public Institution InstitutionBaseInfo { get; set; }

        //users working for institution (principal, teachers)
        [DataMember]
        public IEnumerable<long> UserIds { get; set; }

        [DataMember]
        public IEnumerable<long> GroupIds { get; set; }

       
    }
}