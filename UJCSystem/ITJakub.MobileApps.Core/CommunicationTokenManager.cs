using System;

namespace ITJakub.MobileApps.Core
{
    public class CommunicationTokenManager
    {
        private readonly TimeSpan m_timeToTokenExpiration;

        public CommunicationTokenManager(TimeSpan timeToTokenExpiration)
        {
            m_timeToTokenExpiration = timeToTokenExpiration;
        }

        public string CreateNewToken()
        {
            return Guid.NewGuid().ToString();
        }

        public DateTime GetExpirationTime(DateTime communicationTokenCreateTime)
        {
            return communicationTokenCreateTime.Add(m_timeToTokenExpiration);
        }

        public bool IsCommunicationTokenActive(DateTime communicationTokenCreateTime)
        {
            return GetExpirationTime(communicationTokenCreateTime) >= DateTime.UtcNow;
        }
    }
}