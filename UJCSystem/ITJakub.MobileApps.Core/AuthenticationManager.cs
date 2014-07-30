using System;
using Castle.MicroKernel;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core
{
    public class AuthenticationManager
    {
        private readonly UserRepository m_userRepository;
        private readonly TimeSpan m_timeToTokenExpiration;

        public AuthenticationManager(IKernel container,int timeToTokenExpiration)
        {
            m_userRepository = container.Resolve<UserRepository>();
            m_timeToTokenExpiration = new TimeSpan(0, 0, timeToTokenExpiration);
        }


        public void AuthenticateByCommunicationToken(string communicationToken)
        {
            if (!m_userRepository.IsCommunicationTokenValid(communicationToken, m_timeToTokenExpiration)) throw new Exception("Recieved token expired or is not valid. Login again please..."); //TODO throw better exception
        }

        public void AuthenticateByProvider(string email, string accessToken, AuthenticationProviders authenticationProvider)
        {
            throw new System.NotImplementedException();
        }
    }
}