using System.Runtime.Serialization;

namespace ITJakub.MobileApps.MobileContracts
{
    [DataContract]
    public enum SearchDestinationContract
    {
        [EnumMember]
        Author,

        [EnumMember]
        Title
    }
}