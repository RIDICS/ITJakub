using System;

namespace ITJakub.MobileApps.Client.Core.Manager
{
    public class LoginResult
    {
        public string CommunicationToken { get; set; }
        public DateTime EstimatedExpirationTime { get; set; }
    }
}