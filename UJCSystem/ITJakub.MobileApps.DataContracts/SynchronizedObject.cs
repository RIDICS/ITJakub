using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class SynchronizedObject
    {
        [DataMember]
        public string Data;
    }
}