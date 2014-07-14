using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.DataContracts
{
    [ServiceContract]
    public interface IMobileAppsService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", 
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json, 
            UriTemplate = "json/{test}")]
        string TestMethod(string test);
    }
}