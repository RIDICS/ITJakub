using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class AddUserToRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly DefaultUserProvider m_defaultUserProvider;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly int m_userId;
        private readonly int m_roleId;

        public AddUserToRoleWork(PermissionRepository permissionRepository, DefaultUserProvider defaultUserProvider,
            CommunicationProvider communicationProvider, int userId, int roleId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_defaultUserProvider = defaultUserProvider;
            m_communicationProvider = communicationProvider;
            m_userId = userId;
            m_roleId = roleId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var role = m_defaultUserProvider.GetDefaultUnregisteredRole();
            if (role.Id == m_roleId)
            {
                throw new ArgumentException($"Users cannot be added to the default role {role.Name}");
            }

            var group = m_permissionRepository.FindGroupByExternalIdOrCreate(m_roleId);
            var user = m_permissionRepository.GetUserWithGroups(m_userId);
            if (user.ExternalId == null)
            {
                throw new ArgumentException($"User with ID {user.Id} has missing ExternalID");
            }

            if (user.Groups == null)
            {
                user.Groups = new List<UserGroup>();
            }

            // Assign group to user (fetch lower amount of data)

            user.Groups.Add(group);
            m_permissionRepository.Save(user);
            m_permissionRepository.Flush();


            var client = m_communicationProvider.GetAuthUserApiClient();
            client.AddRoleToUserAsync(user.ExternalId.Value, m_roleId).GetAwaiter().GetResult();
        }
    }
}