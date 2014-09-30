using System.Runtime.Serialization;
using System.ServiceModel;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class ApplicationNotRunningFault : FaultException
    {
        
    }
}