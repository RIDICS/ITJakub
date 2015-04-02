using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Groups
{
    [DataContract]
    public enum GroupStateContract : short
    {
        [EnumMember]
        Created = 0,
        [EnumMember]
        AcceptMembers = 1,
        [EnumMember]
        WaitingForStart = 2,
        [EnumMember]
        Running = 3,
        [EnumMember]
        Paused = 4,
        [EnumMember]
        Closed = 5
    }
}