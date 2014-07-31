using System;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Authentication
{
    public class AuthenticationManager
    {
        private readonly AuthProviderDirector m_authDirector;
        private readonly UsersRepository m_usersRepository;
        private readonly TimeSpan m_timeToTokenExpiration;

        public AuthenticationManager(AuthProviderDirector authDirector, UsersRepository usersRepository, TimeSpan timeToTokenExpiration)
        {
            m_authDirector = authDirector;
            m_usersRepository = usersRepository;
            m_timeToTokenExpiration = timeToTokenExpiration;
        }


        public void AuthenticateByCommunicationToken(string communicationToken)
        {
            if (!m_usersRepository.IsCommunicationTokenValid(communicationToken, m_timeToTokenExpiration))
                throw new AuthException("Recieved token expired or is not valid. Login again please...");
        }

        public void AuthenticateByProvider(string email, string accessToken, AuthenticationProviders authenticationProvider)
        {
            var providerEmail = m_authDirector.GetProvider(authenticationProvider).GetEmail(accessToken).ToLower();
            if (!email.ToLower().Equals(providerEmail)) throw new AuthException("Users e-mail is not valid.");
        }
    }
}