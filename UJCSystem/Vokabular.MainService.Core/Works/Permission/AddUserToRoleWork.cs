using System;
using System.Collections.Generic;
using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts;
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
            var group = m_permissionRepository.FindById<UserGroup>(m_roleId);
            var roleGroup = group as RoleUserGroup;
            var role = m_defaultUserProvider.GetDefaultUnregisteredRole();
            if (roleGroup != null && role.Id == roleGroup.ExternalId)
            {
                throw new MainServiceException(MainServiceErrorCode.AddUserToDefaultRole,
                    $"Users cannot be added to the default role {role.Name}",
                    HttpStatusCode.BadRequest,
                    role.Name
                );
            }

            var user = m_permissionRepository.GetUserWithGroups(m_userId);
            if (user.ExternalId == null)
            {
                throw new MainServiceException(MainServiceErrorCode.UserHasMissingExternalId,
                    $"User with ID {user.Id} has missing ExternalID",
                    HttpStatusCode.BadRequest,
                    user.Id
                );
            }

            if (user.Groups == null)
            {
                user.Groups = new List<UserGroup>();
            }

            // Assign group to user (fetch lower amount of data)

            user.Groups.Add(group);
            m_permissionRepository.Save(user);
            m_permissionRepository.Flush();


            if (roleGroup != null)
            {
                var client = m_communicationProvider.GetAuthUserApiClient();
                client.AddRoleToUserAsync(user.ExternalId.Value, roleGroup.ExternalId).GetAwaiter().GetResult();
            }
            else
            {
                throw new InvalidOperationException($"Only RoleUserGroup can be updated by this method, argument type was: {group.GetType()}");
            }
        }
    }
}