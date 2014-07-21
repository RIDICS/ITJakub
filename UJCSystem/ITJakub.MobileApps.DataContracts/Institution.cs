using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class Institution
    {
        [DataMember]
        public string Name { get; set; }

    }
}