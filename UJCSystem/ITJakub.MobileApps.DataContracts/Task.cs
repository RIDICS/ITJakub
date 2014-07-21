using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class Task
    {
        [DataMember]
        public string Data;
    }
}