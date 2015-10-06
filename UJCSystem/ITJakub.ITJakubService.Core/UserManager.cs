using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts;
using log4net;

namespace ITJakub.ITJakubService.Core
{
    public class UserManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CommunicationTokenGenerator m_communicationTokenGenerator;
        private readonly DefaultUserProvider m_defaultMembershipProvider;
        private readonly UserRepository m_userRepository;

        public UserManager(UserRepository userRepository, CommunicationTokenGenerator communicationTokenGenerator,
            DefaultUserProvider defaultMembershipProvider)
        {
            m_userRepository = userRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
            m_defaultMembershipProvider = defaultMembershipProvider;
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
                CommunicationToken = m_communicationTokenGenerator.GetNewCommunicationToken(),
                CommunicationTokenCreateTime = now,
                Groups = new List<Group> {m_defaultMembershipProvider.GetDefaultGroup()}
            };
            var userId = m_userRepository.Create(dbUser);
            return GetUserDetail(userId);
        }

        public UserContract FindByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                var message = "Username could not be empty";

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


        public User GetCurrentUser()
        {
            if (ServiceSecurityContext.Current != null)
            {
                var username = ServiceSecurityContext.Current.PrimaryIdentity.Name;
                return m_userRepository.GetByLogin(username);
            }

            return m_defaultMembershipProvider.GetDefaultUser();
        }

        public string GetCurrentUserName()
        {
            if (ServiceSecurityContext.Current != null)
            {
                var username = ServiceSecurityContext.Current.PrimaryIdentity.Name;
                return username;
            }

            return m_defaultMembershipProvider.GetDefaultUserName();
        }
    }
}