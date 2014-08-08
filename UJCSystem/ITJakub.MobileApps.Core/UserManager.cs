using System;
using AutoMapper;
using ITJakub.MobileApps.Core.Authentication;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Entities;
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
        public void CreateUser(AuthProvidersContract provider, string providerToken, UserDetailContract userDetail)
        {
            DateTime now = DateTime.UtcNow;
            User user = Mapper.Map<UserDetailContract, User>(userDetail);
            user.AuthenticationProvider = Mapper.Map<AuthProvidersContract, AuthenticationProviders>(provider);
            user.CreateTime = now;
            user.CommunicationToken = m_tokenManager.CreateNewToken();
            user.CommunicationTokenCreateTime = now;


            if (provider != AuthProvidersContract.ItJakub)
            {
                m_authenticationManager.AuthenticateUserAccount(provider, providerToken, user);
            }


            m_userRepository.Create(user);
        }

        public LoginUserResponse LoginUser(AuthProvidersContract provider, string providerToken, string email)
        {
            AuthenticationProviders providerType = Mapper.Map<AuthProvidersContract, AuthenticationProviders>(provider);
            User user = m_userRepository.FindByEmailAndProvider(email, providerType);
            DateTime now = DateTime.UtcNow;
            m_authenticationManager.AuthenticateUserAccount(provider, providerToken, user);

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

        private UserRoleContract GetUserRoleForUser(User user)
        {
            return user.Institution == null ? UserRoleContract.Student : UserRoleContract.Teacher;
        }
    }
}