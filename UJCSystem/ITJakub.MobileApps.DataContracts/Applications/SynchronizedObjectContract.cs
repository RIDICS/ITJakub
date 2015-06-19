using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Applications
{
    [DataContract]
    public class SynchronizedObjectContract
    {
        [DataMember]
        public string ObjectType { get; set; }

        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public SynchronizationTypeContract SynchronizationType { get; set; }
    }

    [DataContract]
    public enum SynchronizationTypeContract
    {
        [EnumMember]
        HistoryTrackingObject = 0,

        [EnumMember]
        SingleObject = 1
    }
}
