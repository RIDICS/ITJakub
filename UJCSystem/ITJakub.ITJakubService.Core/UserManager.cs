using System;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class UserManager
    {
        private readonly UserRepository m_userRepository;

        public UserManager(UserRepository userRepository)
        {
            m_userRepository = userRepository;
        }

        public UserContract CreateLocalUser(UserContract user)
        {
            var now = DateTime.UtcNow;
            var dbUser = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreateTime = now,
                PasswordHash = user.PasswordHash,
                AuthenticationProvider = AuthenticationProvider.ItJakub,
                CommunicationToken = Guid.NewGuid().ToString(), //TODO this should do communicationTokenManager
                CommunicationTokenCreateTime = now
            };
            var userId = m_userRepository.Create(dbUser);
            return FindById(userId);
        }

        public UserContract FindByUserName(string userName)
        {
            var dbUser = m_userRepository.FindByUserName(userName);
            if (dbUser == null) return null;
            var user = Mapper.Map<UserContract>(dbUser);
            return user;
        }

        public UserContract FindById(int userId)
        {
            var dbUser = m_userRepository.FindById(userId);
            if (dbUser == null) return null;
            var user = Mapper.Map<UserContract>(dbUser);
            return user;
        }
    }
}