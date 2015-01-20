using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Groups
{
    [DataContract]
    public class CreateGroupResponse
    {
        [DataMember]
        public string EnterCode { get; set; }

        [DataMember]
        public long GroupId { get; set; }
    }
}