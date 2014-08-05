using Windows.Security.Authentication.Web;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker
{
    public class AuthBrokerResult
    {
        public string ResponseData { get; set; }
        public WebAuthenticationStatus ResponseStatus { get; set; }
    }
}