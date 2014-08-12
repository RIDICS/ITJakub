using System.Net;
using System.ServiceModel.Web;
using ITJakub.MobileApps.Core.Authentication.Providers;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Authentication
{
    public class AuthenticationManager
    {
        private readonly AuthProviderDirector m_authDirector;
        private readonly CommunicationTokenManager m_communicationTokenManager;
        private readonly UsersRepository m_usersRepository;

        public AuthenticationManager(AuthProviderDirector authDirector, UsersRepository usersRepository,
            CommunicationTokenManager communicationTokenManager)
        {
            m_authDirector = authDirector;
            m_usersRepository = usersRepository;
            m_communicationTokenManager = communicationTokenManager;
        }


        public void AuthenticateByCommunicationToken(string communicationToken, UserRoleContract minRoleContractAllowed = UserRoleContract.Student,
            long? requiredUserId = null)
        {
            User user = m_usersRepository.GetUserByCommunicationToken(communicationToken);

            if (user == null || !m_communicationTokenManager.IsCommunicationTokenActive(user.CommunicationTokenCreateTime))
                throw new WebFaultException(HttpStatusCode.Unauthorized)
                {
                    Source = "Recieved token expired or is not valid."
                };

            if (minRoleContractAllowed.Equals(UserRoleContract.Teacher) && user.Institution == null)
                throw new WebFaultException(HttpStatusCode.Unauthorized)
                {
                    Source = "You don't have enough privileges"
                };

            if (requiredUserId != null && !user.Id.Equals(requiredUserId))
                throw new WebFaultException(HttpStatusCode.Unauthorized)
                {
                    Source = "Recieved token does not belong to recieved user identificator"
                };
        }

        public void AuthenticateByProvider(UserLogin userLoginInfo, User dbUser)
        {
            IAuthProvider authProvider = m_authDirector.GetProvider(userLoginInfo.AuthProviderContract);
            AuthenticateResultInfo authResult = authProvider.Authenticate(userLoginInfo.AuthenticationToken, dbUser.Email);

            if (authResult == null || authResult.Result == AuthResultType.Failed)
                throw new WebFaultException(HttpStatusCode.Unauthorized)
                {
                    Source = "Users e-mail is not valid."
                };


            dbUser.AvatarUrl = authResult.UserImageLocation;

            if (authProvider.IsExternalProvider)
                dbUser.AuthenticationProviderToken = userLoginInfo.AuthenticationToken;
        }

        public void AuthenticateUserAccount(AuthProvidersContract provider, string providerToken, User user)
        {
            IAuthProvider authProvider = m_authDirector.GetProvider(provider);
            AuthenticateResultInfo result = authProvider.Authenticate(providerToken, user.Email);

            if (result == null || result.Result == AuthResultType.Failed)
                throw new WebFaultException(HttpStatusCode.Unauthorized)
                {
                    Source = "Users e-mail is not valid."
                };


            user.AvatarUrl = result.UserImageLocation;


            if (authProvider.IsExternalProvider)
                user.AuthenticationProviderToken = providerToken;
        }
    }
}