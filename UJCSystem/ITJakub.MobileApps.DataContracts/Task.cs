using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class Task
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Data { get; set; }
    }
}