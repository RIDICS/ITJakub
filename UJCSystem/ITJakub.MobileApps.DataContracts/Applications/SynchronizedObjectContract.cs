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
    }
}
