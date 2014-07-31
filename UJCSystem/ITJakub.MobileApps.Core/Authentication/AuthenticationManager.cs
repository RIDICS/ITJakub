using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Authentication
{
    public class AuthenticationManager
    {
        private readonly AuthProviderDirector m_authDirector;
        private readonly CommunicationTokenManager m_communicationTokenManager;
        private readonly UsersRepository m_usersRepository;

        public AuthenticationManager(AuthProviderDirector authDirector, UsersRepository usersRepository, CommunicationTokenManager communicationTokenManager)
        {
            m_authDirector = authDirector;
            m_usersRepository = usersRepository;
            m_communicationTokenManager = communicationTokenManager;
        }


        public void AuthenticateByCommunicationToken(string communicationToken)
        {
            var user = m_usersRepository.GetUserByCommunicationToken(communicationToken);
            if (user == null || !m_communicationTokenManager.IsCommunicationTokenActive(user.CommunicationTokenCreateTime))
                throw new AuthException("Recieved token expired or is not valid. Login again please...");
        }

        public void AuthenticateByProvider(string email, string authenticationToken, AuthenticationProviders authenticationProvider)
        {
            if (!m_authDirector.GetProvider(authenticationProvider).Authenticate(authenticationToken, email))
                throw new AuthException("Users e-mail is not valid.");
        }
    }
}