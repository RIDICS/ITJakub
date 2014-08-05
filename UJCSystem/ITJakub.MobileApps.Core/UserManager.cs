using System;
using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core
{
    public class UserManager
    {
        private readonly UsersRepository m_userRepository;
        private readonly CommunicationTokenManager m_tokenManager;

        public UserManager(UsersRepository userRepository,CommunicationTokenManager tokenManager)
        {
            m_userRepository = userRepository;
            m_tokenManager = tokenManager;
        }

        public void CreateRemoteUserAccount(User user, AuthenticationProviders authProvider, string authenticationProviderToken)
        {
            DataEntities.Database.Entities.User deUser = CreateUserSkeleton(user, authProvider);
            deUser.AuthenticationProviderToken = authenticationProviderToken;
            try
            {
                m_userRepository.Create(deUser);
            }
            catch (CreateEntityFailedException)
            {
            }
        }

        public void CreateLocalUserAccount(UserWithSalt user)
        {
            DataEntities.Database.Entities.User deUser = CreateUserSkeleton(user, AuthenticationProviders.ItJakub);
            deUser.PasswordHash = user.PasswordHash;
            deUser.Salt = user.Salt;
            try
            {
                m_userRepository.Create(deUser);
            }
            catch (CreateEntityFailedException)
            {
            }
        }


        private DataEntities.Database.Entities.User CreateUserSkeleton(User user, AuthenticationProviders provider)
        {
            var deUser = Mapper.Map<DataEntities.Database.Entities.User>(user);
            deUser.CreateTime = DateTime.UtcNow;
            deUser.AuthenticationProvider = (byte) provider;
            deUser.CommunicationToken = m_tokenManager.CreateNewToken();
            deUser.CommunicationTokenCreateTime = deUser.CreateTime;
            return deUser;
        }

        public LoginUserResponse Login(UserLogin userLogin)
        {
            DataEntities.Database.Entities.User user = m_userRepository.FindByEmailAndProvider(userLogin.Email, (byte) userLogin.AuthenticationProvider);
            if (!userLogin.AuthenticationProvider.Equals(AuthenticationProviders.ItJakub)) user.AuthenticationProviderToken = userLogin.AuthenticationToken; //actualize access token
            user.CommunicationToken = m_tokenManager.CreateNewToken(); //on every login generate new token
            user.CommunicationTokenCreateTime = DateTime.UtcNow;
            m_userRepository.Update(user);
            return new LoginUserResponse {UserId = user.Id, CommunicationToken = user.CommunicationToken, EstimatedExpirationTime = m_tokenManager.GetExpirationTime(user.CommunicationTokenCreateTime)};
        }

        public void CreateAccount(string authenticationProvider, string authenticationProviderToken, User user)
        {
            var authProvider = (AuthenticationProviders) Enum.Parse(typeof (AuthenticationProviders), authenticationProvider);
            if (authProvider.Equals(AuthenticationProviders.ItJakub))
                CreateLocalUserAccount((UserWithSalt) user);
            else
                CreateRemoteUserAccount(user, authProvider, authenticationProviderToken);
        }
    }
}