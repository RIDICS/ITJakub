using System.Runtime.Serialization;

namespace ITJakub.MobileApps.MobileContracts
{
    [DataContract]
    public class PageContract
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public string XmlId { get; set; }
    }
}