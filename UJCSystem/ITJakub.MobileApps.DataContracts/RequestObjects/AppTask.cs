using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.RequestObjects
{
    [DataContract]
    public class AppTask
    {
        [DataMember]
        public string Data;
    }
}