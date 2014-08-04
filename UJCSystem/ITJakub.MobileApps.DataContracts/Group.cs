using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class Group
    {
        [DataMember]
        public string Name { get; set; }
    }
}