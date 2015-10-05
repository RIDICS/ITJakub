using System;
using ITJakub.DataEntities.Database.Repositories;

namespace ITJakub.ITJakubService.Core
{
    public class AuthenticationManager
    {
        private readonly UserRepository m_userRepository;
        private readonly TimeSpan m_timeToTokenExpiration;

        public AuthenticationManager(UserRepository userRepository, TimeSpan timeToTokenExpiration)
        {
            m_userRepository = userRepository;
            m_timeToTokenExpiration = timeToTokenExpiration;
        }

        public bool ValidateUserAndCommToken(string userName, string commToken)
        {
            var now = DateTime.UtcNow;
            var user = m_userRepository.GetByLogin(userName);
            if (user != null && user.CommunicationToken != null && user.CommunicationToken == commToken && ((user.CommunicationTokenCreateTime + m_timeToTokenExpiration) >= now))
                return true;
            return false;
        }
    }
}