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

        public void CreateRemoteUserAccount(UserDetailContract userDetailContract, AuthenticationProviders authProvider, string authenticationProviderToken)
        {
            DataEntities.Database.Entities.User deUser = CreateUserSkeleton(userDetailContract, authProvider);
            deUser.AuthenticationProviderToken = authenticationProviderToken;
            try
            {
                m_userRepository.Create(deUser);
            }
            catch (CreateEntityFailedException)
            {
            }
        }

        public void CreateLocalUserAccount(UserDetailContractWithSalt userDetailContract)
        {
            DataEntities.Database.Entities.User deUser = CreateUserSkeleton(userDetailContract, AuthenticationProviders.ItJakub);
            deUser.PasswordHash = userDetailContract.PasswordHash;
            deUser.Salt = userDetailContract.Salt;
            try
            {
                m_userRepository.Create(deUser);
            }
            catch (CreateEntityFailedException)
            {
            }
        }


        private DataEntities.Database.Entities.User CreateUserSkeleton(UserDetailContract userDetailContract, AuthenticationProviders provider)
        {
            var deUser = Mapper.Map<DataEntities.Database.Entities.User>(userDetailContract);
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

        public void CreateAccount(string authenticationProviderToken, AuthenticationProviders authenticationProvider, UserDetailContract userDetailContract)
        {
            var authProvider = authenticationProvider;
            if (authProvider == AuthenticationProviders.ItJakub)
                CreateLocalUserAccount((UserDetailContractWithSalt) userDetailContract);
            else
                CreateRemoteUserAccount(userDetailContract, authProvider, authenticationProviderToken);
        }

        private UserRole GetUserRoleForUser(DataEntities.Database.Entities.User user)
        {
            return user.Institution == null ? UserRole.Student : UserRole.Teacher;
        }
    }
}