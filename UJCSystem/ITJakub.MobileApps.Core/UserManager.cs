using System;
using System.ServiceModel.Web;
using AutoMapper;
using ITJakub.MobileApps.Core.Authentication;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core
{
    public class UserManager
    {
        private readonly AuthenticationManager m_authenticationManager;
        private readonly CommunicationTokenManager m_tokenManager;
        private readonly UsersRepository m_userRepository;

        public UserManager(UsersRepository userRepository, CommunicationTokenManager tokenManager, AuthenticationManager authenticationManager)
        {
            m_userRepository = userRepository;
            m_tokenManager = tokenManager;
            m_authenticationManager = authenticationManager;
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
            var user = m_userRepository.FindByEmailAndProvider(userLogin.Email,
                (byte) userLogin.AuthenticationProvider);
            var now = DateTime.UtcNow;
            try
            {
                m_authenticationManager.AuthenticateByProvider(userLogin, user);

                user.CommunicationToken = m_tokenManager.CreateNewToken(); //on every login generate new token
                user.CommunicationTokenCreateTime = now;

                m_userRepository.Update(user);

                return new LoginUserResponse
                {
                    UserId = user.Id,
                    CommunicationToken = user.CommunicationToken,
                    EstimatedExpirationTime = m_tokenManager.GetExpirationTime(user.CommunicationTokenCreateTime),
                    ProfilePictureUrl = user.AvatarUrl,
                    UserRole = GetUserRoleForUser(user)
                };
            }
            catch (WebFaultException)
            {
                throw;
            }
        }

        public void CreateAccount(string authenticationProviderToken, AuthenticationProviders authenticationProvider, User user)
        {
            var authProvider = authenticationProvider;
            if (authProvider == AuthenticationProviders.ItJakub)
                CreateLocalUserAccount((UserWithSalt) user);
            else
                CreateRemoteUserAccount(user, authProvider, authenticationProviderToken);
        }

        private UserRole GetUserRoleForUser(DataEntities.Database.Entities.User user)
        {
            return user.Institution == null ? UserRole.Student : UserRole.Teacher;
        }
    }
}