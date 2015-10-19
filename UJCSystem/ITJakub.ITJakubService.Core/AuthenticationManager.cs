using System;
using System.IdentityModel.Tokens;
using System.Security.Authentication;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using Jewelery;

namespace ITJakub.ITJakubService.Core
{
    public class AuthenticationManager
    {
        private readonly UserRepository m_userRepository;
        private readonly CommunicationTokenGenerator m_communicationTokenGenerator;
        private readonly TimeSpan m_timeToTokenExpiration;

        public AuthenticationManager(UserRepository userRepository, CommunicationTokenGenerator communicationTokenGenerator, TimeSpan timeToTokenExpiration, TimeSpan virtualTokenExpiration)
        {
            m_userRepository = userRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
            m_timeToTokenExpiration = timeToTokenExpiration;
        }

        public void ValidateUserAndCommToken(string userName, string commToken)
        {
            var user = m_userRepository.GetByLoginAndCommToken(userName, commToken);

            if (user == null || user.CommunicationToken == null || user.CommunicationToken != commToken )
                throw new AuthenticationException("Invalid credentials");
        }

        public void ValidateUserAndPassword(string userName, string password)
        {
            var user = m_userRepository.GetByLogin(userName);

            if (user == null || user.PasswordHash == null || (!CustomPasswordHasher.ValidatePassword(password.Split(':')[1], user.PasswordHash)))
                throw new AuthenticationException("Invalid credentials");                                    
        }

        


        public bool RenewCommToken(string userName)
        {
            var now = DateTime.UtcNow;
            var user = m_userRepository.GetByLogin(userName);
            if (user != null)
            {
                user.CommunicationToken = m_communicationTokenGenerator.GetNewCommunicationToken();
                user.CommunicationTokenCreateTime = now;

                m_userRepository.Save(user);
                return true;
            }

            return false;
        }

        public DateTime GetExpirationTimeForToken(User user)
        {
            return user.CreateTime + m_timeToTokenExpiration;
        }
    }
}