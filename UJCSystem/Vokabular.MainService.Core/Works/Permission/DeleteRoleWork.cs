using System;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class DeleteRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly DefaultUserProvider m_defaultUserProvider;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly int m_roleId;

        public DeleteRoleWork(PermissionRepository permissionRepository, DefaultUserProvider defaultUserProvider,
            CommunicationProvider communicationProvider, int roleId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_defaultUserProvider = defaultUserProvider;
            m_communicationProvider = communicationProvider;
            m_roleId = roleId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var unregisteredRole = m_defaultUserProvider.GetDefaultUnregisteredRole();
            if (unregisteredRole.Id == m_roleId)
            {
                throw new ArgumentException($"The default role {unregisteredRole.Name} cannot be deleted.");
            }

            var registeredRole = m_defaultUserProvider.GetDefaultRegisteredRole();
            if (registeredRole.Id == unregisteredRole.Id)
            {
                throw new ArgumentException($"The default role {unregisteredRole.Name} cannot be deleted.");
            }

            var group = m_permissionRepository.FindGroupByExternalId(m_roleId);
            if (group != null)
            {
                m_permissionRepository.Delete(group);
                m_permissionRepository.Flush();
            }

            var client = m_communicationProvider.GetAuthRoleApiClient();
            client.HttpClient.DeleteItemAsync<RoleContract>(m_roleId).GetAwaiter().GetResult();
        }
    }
}