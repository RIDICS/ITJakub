using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using log4net;
using Vokabular.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.Managers
{
    public class UserGroupManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly UserRepository m_userRepository;
        private readonly CommunicationProvider m_communicationProvider;

        public UserGroupManager(UserRepository userRepository, CommunicationProvider communicationProvider)
        {
            m_userRepository = userRepository;
            m_communicationProvider = communicationProvider;
        }

        public List<UserGroupContract> GetGroupsByUser(int userId)
        {
            var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserById(userId));

            if (user == null)
            {
                string message = $"Cannot locate user with id '{userId}'";
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var authUser = client.GetUser(user.ExternalId);
                return Mapper.Map<List<UserGroupContract>>(authUser.Roles);
            }
        }

        public List<UserContract> GetUsersByGroup(int groupId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var members = client.GetUsersByRole(groupId);
                return Mapper.Map<List<UserContract>>(members);
            }
        }

        public int CreateGroup(string groupName, string description)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var roleContract = new RoleContract
                {
                    Description = description,
                    Name = groupName,
                };

                var roleId = client.CreateRole(roleContract);
                return roleId;
            }
        }

        public UserGroupDetailContract GetGroupDetail(int groupId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var role = client.GetRole(groupId);
                if (role == null)
                    return null;

                var members = client.GetUsersByRole(groupId);

                var group = Mapper.Map<UserGroupDetailContract>(role);
                group.Members = Mapper.Map<IList<UserContract>>(members);
                return group;
            }
        }

        public void DeleteGroup(int groupId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                client.DeleteRole(groupId);
            }
        }

        public void RemoveUserFromGroup(int userId, int groupId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                client.RemoveRoleFromUser(userId, groupId);
            }
        }

        public void AddUserToGroup(int userId, int groupId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                client.AddRoleToUser(userId, groupId);
            }
        }

        public List<UserGroupContract> GetUserGroupAutocomplete(string query, int? count)
        {
            if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var result = client.GetListRole(query, countValue);
                return Mapper.Map<List<UserGroupContract>>(result);
            }
        }
    }
}