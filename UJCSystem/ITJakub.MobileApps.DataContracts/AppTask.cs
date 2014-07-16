using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class AppTask
    {
        [DataMember]
        public string Data;
    }
}