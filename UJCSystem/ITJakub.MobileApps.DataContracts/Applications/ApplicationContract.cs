using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Applications
{
    [DataContract]
    public class ApplicationContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}