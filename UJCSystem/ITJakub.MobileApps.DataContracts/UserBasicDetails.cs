using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class UserBasicDetails
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public User User { get; set; }
    }
}