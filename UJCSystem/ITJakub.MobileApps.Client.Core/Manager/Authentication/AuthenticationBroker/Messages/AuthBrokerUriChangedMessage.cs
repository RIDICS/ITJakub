using System;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker.Messages
{

    public class AuthBrokerUriChangedMessage
    {
        public Uri Uri { get; set; }
        public string DocumentTitle { get; set; }
    }

    public class AuthBrokerUriNavigationFailedMessage:AuthBrokerUriChangedMessage
    {
    }
}
