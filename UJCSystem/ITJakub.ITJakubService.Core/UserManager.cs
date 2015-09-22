using System;
using System.Reflection;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.Shared.Contracts;
using log4net;

namespace ITJakub.ITJakubService.Core
{
    public class UserManager
    {
        private readonly UserRepository m_userRepository;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
            return GetUserDetail(userId);
        }

        public UserContract FindByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                string message = "Username could not be empty";

                if (m_log.IsWarnEnabled)
                    m_log.Warn(message);
                throw new ArgumentException(message);
            }

            var dbUser = m_userRepository.FindByUserName(userName);
            if (dbUser == null) return null;
            var user = Mapper.Map<UserContract>(dbUser);
            return user;
        }

        public UserDetailContract GetUserDetail(int userId)
        {
            var dbUser = m_userRepository.FindByIdWithDetails(userId);
            if (dbUser == null) return null;
            var user = Mapper.Map<UserDetailContract>(dbUser);
            return user;
        }
    }
}