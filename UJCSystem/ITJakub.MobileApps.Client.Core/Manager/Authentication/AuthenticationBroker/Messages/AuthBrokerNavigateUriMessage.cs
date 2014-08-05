using System;
using Windows.Security.Authentication.Web;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker.Messages
{
    public class AuthBrokerNavigateUriMessage
    {
        public Uri Uri { get; set; }
        public WebAuthenticationOptions Options { get; set; }
    }
}