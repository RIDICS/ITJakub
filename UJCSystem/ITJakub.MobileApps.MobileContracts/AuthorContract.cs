using System.Runtime.Serialization;

namespace ITJakub.MobileApps.MobileContracts
{
    [DataContract]
    public class AuthorContract
    {
        [DataMember]
        public string Name { get; set; }
    }
}