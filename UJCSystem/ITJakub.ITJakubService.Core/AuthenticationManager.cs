using System;
using System.IdentityModel.Tokens;
using System.Security.Authentication;
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

        public void ValidateUserAndCommToken(string userName, string commToken)
        {
            var now = DateTime.UtcNow;
            var user = m_userRepository.GetByLogin(userName);

            if (user == null || user.CommunicationToken == null || user.CommunicationToken != commToken )
                throw new AuthenticationException("Invalid credentials");

            if((user.CommunicationTokenCreateTime + m_timeToTokenExpiration) <= now)
                throw new SecurityTokenValidationException("Invalid Credentials");            
        }
    }
}